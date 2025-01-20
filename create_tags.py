import os
import sys
import torch
from PIL import Image
from transformers import BlipProcessor, BlipForConditionalGeneration, GPT2Tokenizer, GPT2LMHeadModel
from geopy.geocoders import Nominatim
import sys
sys.stdout.reconfigure(encoding='utf-8')

# Загрузка моделей
processor = BlipProcessor.from_pretrained("Salesforce/blip-image-captioning-base")
blip_model = BlipForConditionalGeneration.from_pretrained("Salesforce/blip-image-captioning-base")

gpt2_model = GPT2LMHeadModel.from_pretrained("t-bank-ai/ruDialoGPT-small")
gpt2_tokenizer = GPT2Tokenizer.from_pretrained("t-bank-ai/ruDialoGPT-small")

# Создаем объект геолокатора
geolocator = Nominatim(user_agent="my_app_name/1.0 (https://mywebsite.com)")

BASE_IMAGE_DIR = r"C:\Users\79194\source\repos\Travelog\Travelog\wwwroot\images"

def get_blip_description(image_path):
    image = Image.open(image_path).convert("RGB")
    inputs = processor(images=image, return_tensors="pt")
    outputs = blip_model.generate(
        **inputs,
        max_length=50,
        num_beams=5,
        temperature=0.9,
        top_k=50,
        top_p=0.95,
        do_sample=True
    )
    description = processor.decode(outputs[0], skip_special_tokens=True)
    return description

def get_detailed_location(latitude, longitude):
    try:
        location = geolocator.reverse((latitude, longitude), language='ru')
        if location:
            address = location.raw.get('address', {})
            country = address.get('country', 'Неизвестно')
            city = address.get('city', address.get('village', 'Неизвестно'))
            region = address.get('state', 'Неизвестно')
            return {"country": country, "city": city, "region": region}
        else:
            return {"country": "Неизвестно", "city": "Неизвестно", "region": "Неизвестно"}
    except Exception as e:
        return {"error": str(e)}

def generate_hashtags(text):
    input_ids = gpt2_tokenizer.encode(text, return_tensors="pt")
    attention_mask = torch.ones(input_ids.shape, device=input_ids.device)

    outputs = gpt2_model.generate(input_ids, attention_mask=attention_mask, pad_token_id=gpt2_tokenizer.eos_token_id, max_new_tokens=30, num_beams=5, do_sample=True, top_k=50, top_p=0.95)
    hashtags = gpt2_tokenizer.decode(outputs[0], skip_special_tokens=True)
    hashtags = [tag for tag in hashtags.split() if tag.startswith("#")]
    return list(set(hashtags) | {"#example_tag"} | {"#example_tag2"} | {"#example_tag3"})

def main():
    if len(sys.argv) != 4:
        print("Usage: python create_tags.py <image_filename> <latitude> <longitude>")   
        sys.exit(1)

    image_filename = sys.argv[1]
    latitude = float(sys.argv[2])
    longitude = float(sys.argv[3])

    # Формируем полный путь к изображению
    image_path = os.path.join(BASE_IMAGE_DIR, image_filename)

    if not os.path.exists(image_path):
        print(f"Файл изображения {image_path} не найден.")
        sys.exit(1)

    description = get_blip_description(image_path)

    # Убираем вывод описания и локации
    hashtags = generate_hashtags(description)
    print(' '.join(hashtags))

if __name__ == "__main__":
    main()

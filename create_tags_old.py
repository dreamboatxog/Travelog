import torch
from PIL import Image
from transformers import BlipProcessor, BlipForConditionalGeneration, GPT2Tokenizer, GPT2LMHeadModel
from geopy.geocoders import Nominatim

processor = BlipProcessor.from_pretrained("Salesforce/blip-image-captioning-base")
blip_model = BlipForConditionalGeneration.from_pretrained("Salesforce/blip-image-captioning-base")

gpt2_model = GPT2LMHeadModel.from_pretrained("t-bank-ai/ruDialoGPT-small")
gpt2_tokenizer = GPT2Tokenizer.from_pretrained("t-bank-ai/ruDialoGPT-small")

# Создаем объект геолокатора
geolocator = Nominatim(user_agent="my_app_name/1.0 (https://mywebsite.com)")

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

    prompt = f"На основе описания изображения, сгенерировать уникальные и релевантные хештеги. Описание изображения: Красивые горы, снежный пейзаж в Париже"

    input_ids = gpt2_tokenizer.encode(prompt, return_tensors="pt")
    attention_mask = torch.ones(input_ids.shape, device=input_ids.device)
    
   
    outputs = gpt2_model.generate(input_ids, attention_mask=attention_mask, pad_token_id=gpt2_tokenizer.eos_token_id, max_new_tokens=30, num_beams=5, do_sample=True, top_k=50, top_p=0.95)
    
    hashtags = gpt2_tokenizer.decode(outputs[0], skip_special_tokens=True)
    
   
    hashtags = [tag for tag in hashtags.split() if tag.startswith("#")]
    
   
    return list(set(hashtags))

def main(image_path, latitude, longitude):
  
    description = get_blip_description(image_path)
    print(f"Описание изображения: {description}")
    
   
    location_info = get_detailed_location(latitude, longitude)
    if "error" in location_info:
        print(f"Ошибка при определении локации: {location_info['error']}")
        return
    print(f"Локация: Страна - {location_info['country']}, Город/Село - {location_info['city']}, Регион - {location_info['region']}")
    
   
    hashtags = generate_hashtags(description)
    print(f"Сгенерированные хештеги: {' '.join(hashtags)}")

# Пример использования:
image_path = "C:\\Users\\79194\\source\\repos\\Travelog\\Travelog\\wwwroot\\images\\3d240e86-2ccf-4aea-819d-30e001675413.jpg"  # Путь к фотографии
latitude = 48.8566  
longitude = 2.3522  

main(image_path, latitude, longitude)
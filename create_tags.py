import os
import sys
import io
import re
from collections import Counter

import torch
from PIL import Image
from transformers import BlipProcessor, BlipForConditionalGeneration
from deep_translator import GoogleTranslator
import spacy 
from geopy.geocoders import Nominatim

sys.stdout.reconfigure(encoding='utf-8')
sys.stderr.reconfigure(encoding='utf-8')

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

# Загрузка модели BLIP
processor = BlipProcessor.from_pretrained("Salesforce/blip-image-captioning-large")
blip_model = BlipForConditionalGeneration.from_pretrained(
    "Salesforce/blip-image-captioning-large"
)

geolocator = Nominatim(user_agent="my_app_name/1.0 (https://mywebsite.com)")

# Загрузка русской модели spacy
nlp = spacy.load("ru_core_news_sm")

BASE_IMAGE_DIR = (
    r"C:\\Users\\burma\\source\\repos\\Travelog1\\Travelog\\wwwroot\\images"
)


def get_detailed_location(latitude, longitude):
    """Получение местоположения по координатам."""
    try:
        location = geolocator.reverse((latitude, longitude), language="ru")
        if location:
            address = location.raw.get("address", {})

            city = address.get("city") or address.get("town") or address.get("village")
            if city:
                return f"{city}"

            region = address.get("state")
            if region:
                return f"{region}"

            country = address.get("country")
            if country:
                return f"{country}"

        return "Неизвестное место"
    except Exception:
        return "Неизвестное место"



def get_image_description(image_path):
    #print("put")
    #print(image_path)
    """Генерация описания изображения с помощью модели BLIP."""
    image = Image.open(image_path).convert("RGB")
    inputs = processor(images=image, return_tensors="pt")

    outputs = blip_model.generate(
        **inputs,
        max_length=750,
        num_beams=9,
        temperature=0.4,
        top_p=0.99,
        top_k=700,
        do_sample=True,
        repetition_penalty=1.5,
        no_repeat_ngram_size=2,
        early_stopping=False,
        num_return_sequences=8,
    )

    descriptions = []
    for output in outputs:
        description = processor.decode(output, skip_special_tokens=True)
        if description.strip() and not re.search("[а-яА-Я]", description):
            description = GoogleTranslator(source="auto", target="ru").translate(
                description
            )
        descriptions.append(description)

    return " ".join(descriptions)


def is_noun(token):
    """Проверяет, является ли токен существительным."""
    return token.pos_ == "NOUN"


def lemmatize_word(token):
    """Приводит слово к начальной форме."""
    return token.lemma_


def generate_hashtags(combined_description, location):
    """Генерация хэштегов на основе объединенного описания."""
    stop_words = {
        "а", "и", "в", "на", "с", "что", "это", "как", "для", "из", "от",
        "изображение", "план", "фотография", "вид"
    }
    words = re.findall(r"\b\w+\b", combined_description.lower())
    words = [word for word in words if word not in stop_words and len(word) > 2]

    doc = nlp(" ".join(words))
    nouns = [lemmatize_word(token) for token in doc if is_noun(token)]
    nouns = [word for word in nouns if word not in stop_words]
    word_freq = Counter(nouns)
    
    important_words = [word for word, _ in word_freq.most_common(4)]

    hashtags = [f"#{word}" for word in important_words]

    if not re.search(r"неизвест", location, re.IGNORECASE):
        location_tag = f"#{location.replace(' ', '_')}"
        hashtags.append(location_tag)

    return hashtags


def main():
    if len(sys.argv) < 4:
        print(
            "Usage: python create_tags.py <image_filename1> "
            "[image_filename2 ...] <latitude> <longitude>"
        )
        sys.exit(1)

    image_filenames = sys.argv[1:-2]
    latitude = float(sys.argv[-2])
    longitude = float(sys.argv[-1])
    location = get_detailed_location(latitude, longitude)
    #print(location)

    combined_description = ""
    for image_filename in image_filenames:
        #print(image_filename)
        #print(BASE_IMAGE_DIR)
        image_path = os.path.join(BASE_IMAGE_DIR, image_filename)
        #print(image_path)
        if not os.path.exists(image_path):
            print(f"Файл изображения {image_path} не найден.")
            continue

        description = get_image_description(image_path)
        combined_description += description + " "

    hashtags = generate_hashtags(combined_description, location)
    print(f"{' '.join(hashtags)}")


if __name__ == "__main__":
    main()
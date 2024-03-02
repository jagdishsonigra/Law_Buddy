import requests
from bs4 import BeautifulSoup
import pyttsx3
import speech_recognition as sr
import datetime
import socket

engine = pyttsx3.init('sapi5')
voices = engine.getProperty('voices')
engine.setProperty('voice', voices[1].id)

def speak(audio):
    engine.say(audio)
    engine.runAndWait()

def takeCommand():
    r = sr.Recognizer()
    with sr.Microphone() as source:
        print("listening..")
        r.pause_threshold = 0.6
        audio = r.listen(source)

    try:
        print("Recognizing")
        query = r.recognize_google(audio, language='en-US')
        print(f"User Said:{query}")

    except Exception as e:
        speak("can you please repeat it")
        print("can you please repeat it?")
        return "None"
    return query

def web_search(query):
    search_url = f"https://www.google.com/search?q={query}"
    response = requests.get(search_url)
    
    if response.status_code == 200:
        soup = BeautifulSoup(response.text, 'html.parser')
        search_results = soup.select('.tF2Cxc')  # Extracting search results from the HTML
        if search_results:
            return search_results[0].get_text()
    
    return None

def api_search(query):
    api_url = "https://google-bard1.p.rapidapi.com/v1/gemini/gemini-pro"

    headers = {
        "api_key": "AIzaSyBMUxgSgfeHMOybY5fodYqwD4e4_ZWYHdg",
        "text": query,
        "X-RapidAPI-Key": "cddfb1b8c5msh91ce8a510853297p175b46jsn0209443547db",
        "X-RapidAPI-Host": "google-bard1.p.rapidapi.com"
    }

    response = requests.get(api_url, headers=headers)
    if response.status_code == 200:
        response_json = response.json()
        text_field = response_json.get('response', '').replace('**', '') 
        return str(text_field)
    
    return None

def wishMe():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_address = ('192.168.253.67', 1194)
    server_socket.bind(server_address)
    server_socket.listen(1)
    print('Waiting for connection...')
    hour = int(datetime.datetime.now().hour)
    try:
        connection, client_address = server_socket.accept()
        print('Connected:', client_address)
        uname = connection.recv(1024).decode()

        if 0 <= hour < 12:
            speak(f"Good morning {uname} how may I help you!")

        elif 12 <= hour < 18:
            speak(f"Good afternoon {uname} how may I help you!")

        else:
            speak(f"Good evening sir {uname} how may I help you sir")

    finally:
        connection.close()

def network():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_address = ('192.168.253.67', 1194)
    server_socket.bind(server_address)
    server_socket.listen(1)
    print('Waiting for connection...')

    while True:
        global query1

        try:
            connection, client_address = server_socket.accept()
            print('Connected:', client_address)

            try:
                hour = int(datetime.datetime.now().hour)

                if 0 <= hour < 12:
                    wish="Good morning law buddy here how may I help you!"
                    connection.sendall(wish.encode())

                elif 12 <= hour < 18:
                    wish="Good afternoon law buddy here how may I help you!"
                    connection.sendall(wish.encode())

                else:
                    wish="Good evening law buddy here sir how may I help you sir"
                    connection.sendall(wish.encode())

                while True:
                    query1 = connection.recv(1024).decode()
                    if not query1:
                        print("Client disconnected.")
                        break
                    s=' give output in one line'
                    print('Received from Unity client:', query1+s)
                    print('Recognizing....')
                    
                    if query1.lower() == 'bye':
                        print('Client requested to end the connection.')
                        break
                    
                    web_result = web_search(query1+s)

                    if web_result:
                        print('Web Search Result:', web_result)
                        connection.sendall(web_result.encode())
                    else:
                        api_result = api_search(query1+s)
                        
                        if 'who are you' in query1:
                            a="I'm virtual lawyer trained by team martians by Jagdish,Tanmay,Ajit,OM from Marathawada Mitra Mandals Polytechnic Guided By Sir Vikas Solanke"
                            connection.sendall(a.encode())

                        elif 'GoogleGenerativeAI' in api_result:
                            offense='offensive content not allowed'
                            connection.sendall(offense.encode())
                        elif api_result:
                            print('API Search Result:', api_result)
                            connection.sendall(api_result.encode())
                        else:
                            connection.sendall("No relevant information found.".encode())

            finally:
                connection.close()

        except KeyboardInterrupt:
            print("Server terminated by the user.")
            break
        except Exception as e:
            print(f"Error: {e}")

if __name__ == "__main__":
    wishMe()
    network()

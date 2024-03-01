import pyttsx3
import speech_recognition as sr
import datetime

engine = pyttsx3.init('sapi5')
voices = engine.getProperty('voices')
print(voices[0].id)
engine.setProperty('voice',voices[1].id)

def speak(audio):
      engine.say(audio)
      engine.runAndWait()

def takeCommand():
    r=sr.Recognizer()
    with sr.Microphone() as source:
            print("listening..")
            r.pause_threshold = 0.6
            audio =r.listen(source)

    try:
          print("Recognizing")
          query = r.recognize_google(audio, language='en-US')
          print(f"User Said:{query}")
    
    except Exception as e:
          speak("can you please repeat it")
          print("can you please repeat it?")
          return "None"
    return query

def wishMe():

      hour = int(datetime.datetime.now().hour)
      if hour>=0 and hour<=12:
            speak(f"Good morning {uname} how may i help you!")

      elif hour>=12 and hour<18:
            speak(f"Good afternoon {uname} how may i help you!")

      else:
            speak(f"Good evening sir {uname} how may i help u sir")

def acceptUser():

      global uname
      speak("please enter ur name")
      uname = input("Please enter your name:")
      
    
if __name__ == "__main__":
      
      acceptUser()
      wishMe()
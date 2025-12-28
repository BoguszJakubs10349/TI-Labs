Lab06 – Notatki tagi i wyszukiwanie

Uruchomienie projektu

1. Utwórz bazę danych TI_Lab w Microsoft SQL Server
2. Wykonaj skrypt SQL tworzący tabele Notes Tags oraz NoteTags
3. Otwórz projekt w Visual Studio
4. Uruchom aplikację
5. Swagger dostępny pod adresem [https://localhost:PORT/swagger](https://localhost:PORT/swagger)
6. Interfejs użytkownika dostępny pod adresem [https://localhost:PORT/notes.html](https://localhost:PORT/notes.html)

Technologie
ASP.NET Core Web API
Microsoft SQL Server
Swagger
HTML + JavaScript

Baza danych
Nazwa bazy danych: TI_Lab

Wymagane tabele
Notes
Tags
NoteTags

Funkcjonalności

Notatki
Dodawanie notatki z tytułem oraz treścią
Automatyczne ustawienie daty utworzenia
Pobieranie listy notatek
Wyszukiwanie notatek po tytule lub treści

Tagi
Lista wszystkich dostępnych tagów
Tworzenie nowych tagów automatycznie podczas przypisywania
Przypisywanie wielu tagów do jednej notatki

Relacje
Relacja wiele do wielu pomiędzy notatkami i tagami
Unikalność przypisania tagu do notatki wymuszona w bazie danych

Endpointy API

GET /api/notes
POST /api/notes

POST /api/notes/{id}/tags

GET /api/tags

Opcjonalnie
GET /api/notes?tag=TAG

Testy ręczne

Dodaj nową notatkę
Dodaj do niej kilka tagów
Sprawdź czy ten sam tag nie może zostać przypisany wielokrot

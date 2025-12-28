Lab03 – Blog komentarze z moderacją

Uruchomienie projektu

1. Utwórz bazę danych TI_Lab w Microsoft SQL Server
2. Wykonaj skrypt SQL tworzący tabele Posts oraz Comments
3. Otwórz projekt w Visual Studio
4. Uruchom aplikację
5. Swagger dostępny pod adresem [https://localhost:PORT/swagger](https://localhost:PORT/swagger)
6. Interfejs użytkownika dostępny pod adresem [https://localhost:PORT/blog.html](https://localhost:PORT/blog.html)

Technologie
ASP.NET Core Web API
Microsoft SQL Server
Swagger
HTML + JavaScript

Baza danych
Nazwa bazy danych: TI_Lab

Wymagane tabele:
Posts
Comments

Funkcjonalności

Posty
Dodawanie posta z tytułem oraz treścią
Pobieranie listy postów
Pobieranie szczegółów posta

Komentarze
Dodawanie komentarza do wybranego posta
Nowy komentarz zapisywany jest jako niezatwierdzony
Pobieranie komentarzy zatwierdzonych dla posta

Moderacja
Zatwierdzanie komentarza przez endpoint moderatorski
Po zatwierdzeniu komentarz jest widoczny w widoku publicznym

Endpointy API

GET /api/posts
POST /api/posts

GET /api/posts/{id}/comments
POST /api/posts/{id}/comments

POST /api/comments/{id}/approve

Testy ręczne

Dodaj nowy post
Dodaj komentarz do posta
Sprawdź że komentarz nie jest widoczny publicznie
Zatwierdź komentarz przez endpoint approve
Odśwież widok komentarzy i sprawdź że komentarz jest widoczny

Uwagi
Widok publiczny zwraca wyłącznie komentarze zatwierdzone
Operacja zatwierdzania komentarza natychmiast wpływa na widok użytkownika

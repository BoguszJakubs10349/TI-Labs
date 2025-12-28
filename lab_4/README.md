Lab04 – Filmy i oceny

Uruchomienie projektu

1. Utwórz bazę danych TI_Lab w Microsoft SQL Server
2. Wykonaj skrypt SQL tworzący tabele Movies oraz Ratings i widok rankingowy
3. Otwórz projekt w Visual Studio
4. Uruchom aplikację
5. Swagger dostępny pod adresem [https://localhost:PORT/swagger](https://localhost:PORT/swagger)
6. Interfejs użytkownika dostępny pod adresem [https://localhost:PORT/movies.html](https://localhost:PORT/movies.html)

Technologie
ASP.NET Core Web API
Microsoft SQL Server
Swagger
HTML + JavaScript

Baza danych
Nazwa bazy danych: TI_Lab

Wymagane tabele
Movies
Ratings

Dodatkowo wykorzystywany jest widok rankingowy vMoviesRanking.

Funkcjonalności

Filmy
Dodawanie filmu z tytułem oraz rokiem produkcji
Pobieranie listy filmów

Oceny
Dodawanie oceny filmu w zakresie od 1 do 5
Walidacja zakresu oceny po stronie aplikacji oraz bazy danych

Ranking
Wyświetlanie listy filmów z obliczoną średnią ocen
Wyświetlanie liczby oddanych głosów
Sortowanie filmów malejąco według średniej ocen

Endpointy API

GET /api/movies
POST /api/movies

POST /api/ratings

Opcjonalnie
GET /api/movies/top
GET /api/movies?year=YYYY

Testy ręczne

Dodaj nowy film
Dodaj ocenę filmu w zakresie od 1 do 5
Sprawdź czy ranking aktualizuje się bez restartu aplikacji
Sprawdź poprawność średniej ocen oraz liczby głosów

Uwagi
Średnia ocen obliczana jest na podstawie zapisanych głosów
Walidacja zakresu ocen realizowana jest po stronie API oraz bazy danych

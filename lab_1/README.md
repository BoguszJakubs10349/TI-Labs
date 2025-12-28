Lab01 – Wypożyczalnia książek

Uruchomienie projektu

1. Utwórz bazę danych TI_Lab w Microsoft SQL Server
2. Wykonaj skrypt SQL tworzący tabele Members Books oraz Loans
3. Otwórz projekt w Visual Studio
4. Uruchom aplikację
5. Swagger dostępny pod adresem [https://localhost:PORT/swagger](https://localhost:PORT/swagger)
6. Interfejs użytkownika dostępny pod adresem [https://localhost:PORT/index.html](https://localhost:PORT/index.html)

Technologie
ASP.NET Core Web API
Microsoft SQL Server
Swagger
HTML + JavaScript

Baza danych
Nazwa bazy danych: TI_Lab

Wymagane tabele:
Members
Books
Loans

Schemat bazy danych zgodny z treścią zadania Lab01 MSSQL Edition.

Funkcjonalności

Czytelnicy
Dodawanie czytelnika z imieniem i adresem email
Adres email musi być unikalny
Pobieranie listy czytelników

Książki
Dodawanie książki z tytułem autorem i liczbą egzemplarzy
Pobieranie listy książek
Wyświetlanie liczby dostępnych egzemplarzy

Wypożyczenia
Wypożyczenie książki przez czytelnika
Automatyczne ustawienie daty wypożyczenia na bieżącą
Automatyczne ustawienie daty zwrotu na 14 dni od wypożyczenia
Zwrot książki
Kontrola dostępności egzemplarzy

Walidacja
Nie można wypożyczyć książki jeśli liczba aktywnych wypożyczeń jest równa lub większa od liczby egzemplarzy
Nie można dodać czytelnika z istniejącym adresem email

Endpointy API

GET /api/members
POST /api/members

GET /api/books
POST /api/books

GET /api/loans
POST /api/loans/borrow
POST /api/loans/return

Testy ręczne

Dodaj nowego czytelnika
Dodaj nową książkę
Wypożycz książkę
Spróbuj wypożyczyć więcej egzemplarzy niż dostępne
Zwróć książkę i sprawdź aktualizację dostępności

Uwagi
Operacja wypożyczenia wykonywana jest w transakcji
Błędy walidacji zwracają odpowiednie kody HTTP

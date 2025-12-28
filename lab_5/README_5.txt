Lab05 – Kanban kolumny i zadania

Uruchomienie projektu

1. Utwórz bazę danych TI_Lab w Microsoft SQL Server
2. Wykonaj skrypt SQL tworzący tabele Columns oraz Tasks oraz inicjalizujący kolumny
3. Otwórz projekt w Visual Studio
4. Uruchom aplikację
5. Swagger dostępny pod adresem [https://localhost:PORT/swagger](https://localhost:PORT/swagger)
6. Interfejs użytkownika dostępny pod adresem [https://localhost:PORT/kanban.html](https://localhost:PORT/kanban.html)

Technologie
ASP.NET Core Web API
Microsoft SQL Server
Swagger
HTML + JavaScript

Baza danych
Nazwa bazy danych: TI_Lab

Wymagane tabele
Columns
Tasks

Funkcjonalności

Kolumny
Trzy predefiniowane kolumny Todo Doing oraz Done
Każda kolumna posiada pole porządkowe ord

Zadania
Dodawanie zadania z tytułem i przypisaniem do kolumny
Automatyczne ustawienie pola ord jako maksymalna wartość plus jeden w danej kolumnie
Wyświetlanie listy zadań w odpowiednich kolumnach

Przenoszenie zadań
Przenoszenie zadania do innej kolumny
Aktualizacja kolumny oraz pozycji zadania
Zachowanie spójnej kolejności zadań w kolumnach

Endpointy API

GET /api/board

POST /api/tasks

POST /api/tasks/{id}/move

Testy ręczne

Dodaj nowe zadanie do kolumny Todo
Sprawdź czy zadanie pojawia się na końcu listy
Przenieś zadanie do kolumny Doing a następnie Done
Odśwież stronę i sprawdź czy układ zadań został zachowany

Uwagi
Pole ord odpowiada za kolejność zadań w kolumnach
Unikalność kombinacji kolumny i pozycji jest wymuszana po stronie bazy danych
Operacje dodawania i przenoszenia wykonywane są w transakcjach

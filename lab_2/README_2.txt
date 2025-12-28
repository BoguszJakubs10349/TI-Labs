Lab02 – Sklep: koszyk i zamówienia

Uruchomienie projektu

1. Utwórz bazę danych TI_Lab w Microsoft SQL Server
2. Wykonaj skrypt SQL tworzący tabele Products, Orders oraz OrderItems
3. Otwórz projekt w Visual Studio
4. Uruchom aplikację
5. Swagger dostępny pod adresem [https://localhost:PORT/swagger](https://localhost:PORT/swagger)
6. Interfejs użytkownika dostępny pod adresem [https://localhost:PORT/shop.html](https://localhost:PORT/shop.html)

Technologie
ASP.NET Core Web API
Microsoft SQL Server
Swagger
HTML + JavaScript

Baza danych
Nazwa bazy danych: TI_Lab

Wymagane tabele:
Products
Orders
OrderItems

Funkcjonalności

Produkty
Dodawanie produktu z nazwą i ceną większą lub równą zero
Pobieranie listy produktów

Koszyk
Dodawanie produktu do koszyka
Zmiana ilości produktu w koszyku
Usuwanie produktu z koszyka
Podgląd aktualnej zawartości koszyka

Zamówienia
Złożenie zamówienia z aktualnej zawartości koszyka
Zapis zamówienia do bazy danych
Zapis pozycji zamówienia z zapisem ceny jako snapshot
Wyliczenie łącznej kwoty zamówienia
Wyczyszczenie koszyka po złożeniu zamówienia

Endpointy API

GET /api/products
POST /api/products

GET /api/cart
POST /api/cart/add
PATCH /api/cart/item
DELETE /api/cart/item/{product_id}

POST /api/checkout


Testy ręczne

Dodaj kilka produktów przez Swagger
Otwórz stronę shop.html i dodaj produkty do koszyka
Zmień ilości produktów w koszyku
Złóż zamówienie
Sprawdź w bazie danych, czy utworzone zostały rekordy w tabelach Orders oraz OrderItems
Sprawdź, czy koszyk został wyczyszczony po checkout

Uwagi
Cena zapisana w OrderItems jest ceną z momentu zakupu i nie zmienia się po późniejszej edycji produktu.
Operacja checkout wykonywana jest w transakcji.

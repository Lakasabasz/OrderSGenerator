# Generator Rozkazów S
TODO: English Version
## Wymagania
- System Windows 10
- Baza danych MySQL lub kompatybilna
## Instalacja
Po rozpakowaniu archiwum zip pobranego z Releases należy skonfigurować bazę danych i wygenerować klucz produktu.
### Konfiguracja bazy danych
1. Uruchom skrypt `script.sql`
2. Uzupełnij tabele:
   1. `INSERT INTO Settings (Post, YearlyMode) VALUES ('LCS Gdańsk', true')` Skrót posterunku, jeśli numeracja rozkazów jest roczna to `true`, jeśli miesięczna `false`
   2. `INSERT INTO Stations (StationName) VALUES('Gdańsk'), ('Pruszcz')` Po przecinku można dodawać kolejne stacje
   3. `INSERT INTO PhysicalLocations (LocationName, StationId) VALUES ('Nastawnia', 1), ('Nastawnia PRG, 2)` Tekst nazywa miejsce instalacji, numer przypisuje lokalizację do stacji. `SELECT * FROM Stations` wyświetla numery stacji i ich nazwy
   4. ``

W tym celu trzeba przygotować plik `config.yml` według wzoru:
```yaml
connections:
- host: localhost         # Host bazy danych
  port: 3306              # Port bazy danych 
  database: database_name # Nazwa bazy danych
  dbUser: database_user   # Login do bazy danych
  dbPassword: passowrd    # Hasło do bazy danych
  locationId: 1           # Id lokalizacji z bazy danych (lokalizacja komputera)
  setupName: Lokalny      # Nazwa, która będzie się wyświetlać przy logowaniu
- host: remote.local      # Opcjonalna konfiguracja drugiego i następnych połączeń
  port: 3310
  database: remote_db
  dbUser: remote_user
  dbPassword: remote_password
  locationId: 2
  setupName: LCS
```
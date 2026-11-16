# Holiday Maker

Konsolapplikation i C# för en resebyråoperatör som söker lediga rum, registrerar kunder och hanterar bokningar mot en MySQL-databas.

## Krav

MySQL 8 i Docker, .NET SDK (10.0.400) och en SQL-klient, till exempel DBeaver.

## Skapa databasen

1. Starta MySQL i Docker.   
    Första gången:
    docker run --name mysql-kurs -e MYSQL_ROOT_PASSWORD=<root-lösenord> -p 127.0.0.1:3306:3306 -d mysql:8.4
    Om containern redan finns:
    docker start mysql-kurs

2. Anslut i DBeaver som root och kör hela skriptet `docs/holiday_maker_er.sql` (Alt + X). Det skapar databasen `holiday_maker` med tabeller, regler (NOT NULL, UNIQUE, CHECK) och testdata (5 boenden, 20 rum, 10 kunder, 25 bokningar).

3. Skapa databasanvändaren som programmet använder (inte root), med minsta behörighet. Byt `DittLosenord` mot ett eget lösenord:
   """sql
   CREATE USER 'holly'@'172.17.0.1' IDENTIFIED BY 'DittLosenord';
   GRANT SELECT, INSERT, UPDATE, DELETE ON holiday_maker.* TO 'holly'@'172.17.0.1';
   """

   (`172.17.0.1` är den adress MySQL i Docker ser anslutningar från på min dator. Den kan vara en annan hos dig.)

## Köra programmet

1. Sätt lösenordet i miljövariabeln `HOLIDAY_DB_PASSWORD` (Windows: `setx HOLIDAY_DB_PASSWORD "DittLosenord"`, och starta sedan om terminalen).

2. Gå till mappen `HolidayMaker` och kör `dotnet run`.

3. Alla val utom sökningen kräver att man har valt en kund (menyval 1).

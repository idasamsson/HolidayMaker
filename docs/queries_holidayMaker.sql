ALTER TABLE bookings 
ADD CONSTRAINT chk_check_out_check_in CHECK (check_out > check_in);

UPDATE bookings b 
SET check_in = '2026-12-08', check_out = '2026-12-06' 
WHERE id = 1;

DELETE FROM accommodation WHERE id = 1;

-- skapa konto
CREATE USER 'holly'@'172.17.0.1' IDENTIFIED BY 'DittLosenord';
GRANT SELECT, INSERT, UPDATE, DELETE ON holiday_maker.* TO 'holly'@'172.17.0.1';

SELECT user, host FROM mysql.user WHERE user = 'holly';
SHOW GRANTS FOR 'holly'@'172.17.0.1';

SELECT COUNT(*) FROM bookings;      -- ska fungera
CREATE TABLE test_x (id INT);       -- ska nekas

SHOW GRANTS FOR 'holly'@'172.17.0.1';

-- vilka rum är lediga?
SELECT a.name AS boende, c.name AS stad, r.room_number AS rumsnummer, r.id AS rumsid, r.capacity AS kapacitet, r.price_per_night AS pris_per_natt
FROM rooms r
JOIN accommodation a ON a.id = r.accommodation_id
JOIN cities c ON c.id = a.city_id
WHERE r.id NOT IN (
	SELECT room_id
	FROM bookings
	WHERE check_out > '2026-12-12'
  		AND check_in < '2026-12-15')
 AND c.name = 'Palma' AND r.capacity >= 2;

-- kolla om ett rum är ledigt - om 0 finns det inga bokningar
SELECT COUNT(*)
FROM bookings 
WHERE room_id = 1 AND check_out > '2026-12-09' AND check_in < '2026-12-12';

-- kolla rummets kapacitet
 SELECT capacity FROM rooms WHERE id = 1;


-- ta fram en lista med alla kunder ink. kontaktuppgifter
 SELECT id, first_name, last_name, email, phone FROM customers;

-- lägg till en ny kund
 INSERT INTO customers (first_name, last_name, email, phone);

-- leta fram en kund utifrån id
SELECT id, first_name, last_name, email, phone FROM customers WHERE id = 1;

-- leta upp pris/natt för ett specifikt rum
SELECT price_per_night
FROM rooms
WHERE id = 1;

-- Leta fram bokningar på ett kund-id
SELECT b.id, a.name, r.room_number, b.check_in, b.check_out, DATEDIFF(b.check_out, b.check_in) AS antal_nätter, b.guest_count, DATEDIFF(b.check_out, b.check_in)*r.price_per_night AS totalpris
FROM bookings b 
JOIN rooms r ON r.id = b.room_id 
JOIN accommodation a  ON a.id = r.accommodation_id 
WHERE b.customer_id = 5
ORDER BY b.check_in;

-- uppdatera en boknings gästantal
UPDATE bookings b
SET guest_count = 2
WHERE b.id = 10 
	AND b.customer_id = 5
	AND 2 <= (SELECT capacity FROM rooms WHERE id = b.room_id);










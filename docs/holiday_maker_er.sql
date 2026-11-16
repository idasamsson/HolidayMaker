CREATE DATABASE IF NOT EXISTS holiday_maker;
USE holiday_maker;

CREATE TABLE `customers`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `first_name` VARCHAR(255) NOT NULL,
    `last_name` VARCHAR(255) NOT NULL,
    `email` VARCHAR(255) NOT NULL,
    `phone` VARCHAR(255) NOT NULL
);
CREATE TABLE `accommodation`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL,
    `pool` BOOLEAN NOT NULL,
    `restaurant` BOOLEAN NOT NULL,
    `beach_distance` BIGINT NOT NULL,
    `city_center_distance` BIGINT NOT NULL,
    `rating` BIGINT NOT NULL,
    `city_id` BIGINT UNSIGNED NOT NULL
);
CREATE TABLE `rooms`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `accommodation_id` BIGINT UNSIGNED NOT NULL,
    `price_per_night` BIGINT NOT NULL,
    `capacity` BIGINT NOT NULL,
    `room_number` BIGINT UNSIGNED NOT NULL
);
CREATE TABLE `cities`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL
);
CREATE TABLE `bookings`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `customer_id` BIGINT UNSIGNED NOT NULL,
    `room_id` BIGINT UNSIGNED NOT NULL,
    `guest_count` BIGINT UNSIGNED NOT NULL,
    `check_in` DATE NOT NULL,
    `check_out` DATE NOT NULL
);
ALTER TABLE
    `accommodation` ADD CONSTRAINT `accommodation_city_id_foreign` FOREIGN KEY(`city_id`) REFERENCES `cities`(`id`);
ALTER TABLE
    `rooms` ADD CONSTRAINT `rooms_accommodation_id_foreign` FOREIGN KEY(`accommodation_id`) REFERENCES `accommodation`(`id`);
ALTER TABLE
    `bookings` ADD CONSTRAINT `bookings_customer_id_foreign` FOREIGN KEY(`customer_id`) REFERENCES `customers`(`id`);
ALTER TABLE
    `bookings` ADD CONSTRAINT `bookings_room_id_foreign` FOREIGN KEY(`room_id`) REFERENCES `rooms`(`id`);
ALTER TABLE rooms
ADD CONSTRAINT uq_room_per_accommodation UNIQUE (accommodation_id, room_number);

ALTER TABLE rooms
ADD CONSTRAINT chk_capacity CHECK (capacity > 0);

ALTER TABLE rooms
ADD CONSTRAINT chk_price_per_night CHECK (price_per_night > 0);

ALTER TABLE bookings
ADD CONSTRAINT chk_guest_count CHECK (guest_count > 0);

ALTER TABLE bookings
ADD CONSTRAINT chk_check_out_check_in CHECK (check_out > check_in);

-- TESTDATA (samma som CSV-filerna i docs/)
-- Ordningen följer de främmande nycklarna: städer, boenden, rum, kunder, bokningar.

INSERT INTO cities (id, name) VALUES
    (1, 'Malaga'),
    (2, 'Palma'),
    (3, 'Split');

INSERT INTO accommodation (id, name, pool, restaurant, beach_distance, city_center_distance, rating, city_id) VALUES
    (1, 'Hotel Sol y Mar',       1, 1,  150, 2500, 4, 1),
    (2, 'Pensionat Casa Blanca', 0, 0,  900,  300, 3, 1),
    (3, 'Hotel Bahia Azul',      1, 1,   50, 4000, 5, 2),
    (4, 'Villa Olivo',           1, 0, 1200, 1500, 4, 2),
    (5, 'Pension Adria',         0, 1,  400,  600, 2, 3);

INSERT INTO rooms (id, accommodation_id, price_per_night, capacity, room_number) VALUES
    (1,  1, 1100, 2, 101),
    (2,  1, 1900, 4, 102),
    (3,  1,  700, 1, 201),
    (4,  1, 1250, 2, 202),
    (5,  2,  500, 1, 1),
    (6,  2,  750, 2, 2),
    (7,  2,  800, 2, 3),
    (8,  2, 1200, 4, 4),
    (9,  3, 1800, 2, 101),
    (10, 3, 2000, 2, 102),
    (11, 3, 3200, 4, 201),
    (12, 3, 1100, 1, 301),
    (13, 4, 2400, 4, 1),
    (14, 4, 2600, 4, 2),
    (15, 4, 1500, 2, 3),
    (16, 4, 1900, 3, 4),
    (17, 5,  450, 1, 10),
    (18, 5,  650, 2, 11),
    (19, 5,  700, 2, 12),
    (20, 5, 1000, 4, 13);

INSERT INTO customers (id, first_name, last_name, email, phone) VALUES
    (1,  'Anna',  'Lindqvist',  'anna.lindqvist@example.com',   '070-555 01 01'),
    (2,  'Erik',  'Johansson',  'erik.johansson@example.com',   '070-555 01 02'),
    (3,  'Maria', 'Nilsson',    'maria.nilsson@example.com',    '070-555 01 03'),
    (4,  'Lars',  'Eriksson',   'lars.eriksson@example.com',    '070-555 01 04'),
    (5,  'Sofia', 'Berg',       'sofia.berg@example.com',       '070-555 01 05'),
    (6,  'Karl',  'Svensson',   'karl.svensson@example.com',    '070-555 01 06'),
    (7,  'Elin',  'Persson',    'elin.persson@example.com',     '070-555 01 07'),
    (8,  'Oskar', 'Gustafsson', 'oskar.gustafsson@example.com', '070-555 01 08'),
    (9,  'Frida', 'Olsson',     'frida.olsson@example.com',     '070-555 01 09'),
    (10, 'Johan', 'Larsson',    'johan.larsson@example.com',    '070-555 01 10');

INSERT INTO bookings (id, customer_id, room_id, guest_count, check_in, check_out) VALUES
    (1,  1,  1,  2, '2026-12-01', '2026-12-05'),
    (2,  2,  1,  2, '2026-12-05', '2026-12-09'),
    (3,  6,  1,  1, '2026-12-12', '2026-12-15'),
    (4,  3,  2,  4, '2026-12-03', '2026-12-10'),
    (5,  4,  2,  3, '2026-12-10', '2026-12-14'),
    (6,  7,  3,  1, '2026-12-02', '2026-12-06'),
    (7,  1,  4,  2, '2026-12-08', '2026-12-12'),
    (8,  2,  4,  2, '2026-12-20', '2026-12-27'),
    (9,  8,  5,  1, '2026-12-01', '2026-12-08'),
    (10, 5,  6,  2, '2026-12-05', '2026-12-07'),
    (11, 5,  6,  2, '2026-12-07', '2026-12-10'),
    (12, 9,  7,  2, '2026-12-15', '2026-12-22'),
    (13, 3,  8,  4, '2026-12-10', '2026-12-17'),
    (14, 10, 9,  2, '2026-12-01', '2026-12-04'),
    (15, 4,  9,  2, '2026-12-04', '2026-12-11'),
    (16, 5,  10, 2, '2026-12-12', '2026-12-19'),
    (17, 2,  11, 4, '2026-12-06', '2026-12-13'),
    (18, 6,  12, 1, '2026-12-14', '2026-12-18'),
    (19, 4,  13, 3, '2026-12-02', '2026-12-09'),
    (20, 1,  14, 3, '2026-12-17', '2026-12-24'),
    (21, 7,  15, 2, '2026-12-03', '2026-12-05'),
    (22, 2,  16, 2, '2026-12-09', '2026-12-16'),
    (23, 10, 17, 1, '2026-12-01', '2026-12-10'),
    (24, 4,  18, 2, '2026-12-05', '2026-12-12'),
    (25, 8,  19, 2, '2026-12-20', '2026-12-23');

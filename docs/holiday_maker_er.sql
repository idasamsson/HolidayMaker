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
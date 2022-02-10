CREATE DATABASE `assets`
DEFAULT CHARACTER SET utf8 
COLLATE utf8_hungarian_ci;


CREATE TABLE `regions` (
  `id` int(12) PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(50) UNIQUE
);

CREATE TABLE `locations` (
  `id` int(12) PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(50),
  `regionId` int(12),
  FOREIGN KEY (`regionId`) REFERENCES `regions`(`id`)
);

CREATE TABLE `users` (
  `id` int(12) PRIMARY KEY AUTO_INCREMENT,
  `username` varchar(50) UNIQUE,
  `password` varchar(32),
  `locationId` int(12),
  `permission` int(1),
  `active` int(1),
  FOREIGN KEY (`locationId`) REFERENCES `locations`(`id`)
);

CREATE TABLE `products` (
  `id` int(12) PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(50) UNIQUE,
  `unitPrice` int(12)
);

CREATE TABLE `stocks` (
  `id` int(12) PRIMARY KEY AUTO_INCREMENT,
  `productId` int(12),
  `quantity` int(12),
  `locationId` int(12),
  FOREIGN KEY (`productId`) REFERENCES `products`(`id`),
  FOREIGN KEY (`locationId`) REFERENCES `locations`(`id`)
);

CREATE TABLE `purchases` (
  `id` int(12) PRIMARY KEY AUTO_INCREMENT,
  `productId` int(12),
  `quantity` int(12),
  `date` datetime,
  `locationId` int(12),
  `userId` int(12),
  FOREIGN KEY (`userId`) REFERENCES `users`(`id`),
  FOREIGN KEY (`productId`) REFERENCES `products`(`id`),
  FOREIGN KEY (`locationId`) REFERENCES `locations`(`id`)
);

CREATE TRIGGER `tr_purchase_after` 
AFTER INSERT ON `purchases` FOR EACH ROW 
UPDATE `stocks` SET `stocks`.`quantity` = `stocks`.`quantity` + NEW.`quantity` 
WHERE `stocks`.`locationId` = NEW.`locationId` 
AND `stocks`.`productId` = NEW.`productId`;

CREATE TRIGGER `tr_purchase_before` 
BEFORE UPDATE ON `purchases` FOR EACH ROW 
UPDATE `stocks` SET `stocks`.`quantity` = `stocks`.`quantity` + (NEW.`quantity` - (
SELECT `quantity` FROM `purchases` 
WHERE `id` = NEW.`id`))
WHERE `stocks`.`locationId` = NEW.`locationId` 
AND `stocks`.`productId` = NEW.`productId`;

CREATE TABLE `sales` (
  `id` int(12) PRIMARY KEY AUTO_INCREMENT,
  `productId` int(12),
  `quantity` int(12),
  `date` datetime,
  `locationId` int(12),
  `userId` int(12),
  FOREIGN KEY (`userId`) REFERENCES `users`(`id`),
  FOREIGN KEY (`productId`) REFERENCES `products`(`id`),
  FOREIGN KEY (`locationId`) REFERENCES `locations`(`id`)
);

CREATE TRIGGER `tr_sold_after` 
AFTER INSERT ON `sales` FOR EACH ROW 
UPDATE `stocks` SET `stocks`.`quantity` = `stocks`.`quantity` - NEW.`quantity` 
WHERE `stocks`.`locationId` = NEW.`locationId` 
AND `stocks`.`productId` = NEW.`productId`;

CREATE TRIGGER `tr_sold_before` 
BEFORE UPDATE ON `sales` FOR EACH ROW 
UPDATE `stocks` SET `stocks`.`quantity` = `stocks`.`quantity` - (NEW.`quantity` - (
SELECT `quantity` FROM `sales` 
WHERE `id` = NEW.`id`))
WHERE `stocks`.`locationId` = NEW.`locationId` 
AND `stocks`.`productId` = NEW.`productId`;

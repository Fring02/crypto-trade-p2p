# Cryptocurrency P2P trade application
***
This application is a microservices-based application which serves as a P2P application for trading cryptocurrency (*For now, only ETH*).
---
>The technologies used for the development:
>
>- **C#**
>- **ASP.NET Core**
>- **MongoDB**
>- **Redis**
>- **PostGreSQL**
>- **SQL Server**
>- **RabbitMQ**
>- **Docker**

The project contains of multiple microservices and each of them will be described below:

## 1. Authorization service

Authorization service is responsible for managing users registration and login using JWT authentication. In case of success, registration and login return JWT access and refresh tokens. After the access token expiration, token needs to be refreshed and this is also provided. In case of sign out, revoke operation is also available. If the user registers for the first time, he has to create a new crypto wallet or load an existing one. Therefore, a message for loading or creating wallet is pushed to the queue in RabbitMQ service.


## 2. Users service

CRUD service for managing users: retrieving, updating and deleting (admin permission only). 


## 3. Lots service

CRUD service for managing lots. Lot is such an entity which is published by users and is available to view. (That lots are similar to those in Binance). Lots can be viewed by filter, updated, deleted, and created. View is done with filter, including lot type, currency and fiat type, supply, price, and etc. 


## 4. Wallets and transfer services

- These are general services in this project. First one is wallets service. Wallets belong to users and there are two types of wallets: main and P2P. P2P wallets are generally used for trading, while main wallets can be used for other purposes. Wallets can be retrieved, including balance, address. Private keys of wallets are retrieved separately. Wallets can be locked and unlocked. Lock is done, if the user is not permitted to trade. Unlock date is also specified, by this date the wallet will be unlocked.

- Second service is transfer service. This service is responsible for the transfer of cryptocurrency from one wallet to another. This is P2P trade. Transfer is done from seller's P2P wallet to buyer's P2P wallet. For now, only ETH transfer is available. Wallet is retrieved from MongoDB database with it's encrypted private key. Wallet's address is defined, and validation of balance comes first. After successful validation, the amount of ETH is transferred to buyer's wallet by its address and the signature of transaction by seller's private key.


## 5. Trade service

This service is responsible for sessions between users - generally P2P trade itself. The session is between buyer and seller. After the session begins, buyer has fixed time to transfer fiat funds. If he refuses, he can cancel the session. Otherwise, buyer confirms session after transferring fiat funds (this is done not in our project). After session confirmation, seller also has to confirm that he received fiat. After the seller's confirmation, the transfer of cryptocurrency is automatically done.



### Contacts:
- [Fring02](https://github.com/Fring02)
- [LinkedIn profile](https://www.linkedin.com/in/sultanbekkhassenov/)

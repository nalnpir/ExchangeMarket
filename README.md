# ExchangeMarket

## Usage
- Change database settings in appsetttings.json
- On a console write dotnet run
- Navigate to /swagger


## Questions
Q: ¿What do you think about using the user ID as the input endpoint?

A: The idea of using a user ID as an endpoint, is wrong. The biggest issue it lies on the ability for anyone to make transactions in name of someone else, even when we use random guid as user ID or POST request its still a huge risk.

Q: ¿How would you improve the transaction to ensure that the user who makes the purchase is the correct user?

A: Well as I stated previously, right now the usage of an User ID is a security risk. This security flaw could be corrected by the implementation of authentication. When we can prove the user is who he says he is, there also no more need to use an ID, since all that we need is contained in the Cookie, Header, or State of the browser which the user will send for us to validate

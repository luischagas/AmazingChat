dotnet test AmazingChat.Tests

docker-compose build

docker-compose up -d

sleep 5

URL='http://localhost:8080'

if start; then
    start $URL
else
    open $URL
fi
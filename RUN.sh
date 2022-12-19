dotnet test AmazingChat.Tests

docker-compose build

docker-compose up -d

URL=$(docker inspect --format='http://localhost:{{(index (index .NetworkSettings.Ports "80/tcp") 0).HostPort}}' amazing-chat)

start $URL
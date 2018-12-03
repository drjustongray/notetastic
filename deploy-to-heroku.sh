#!/bin/bash

IMAGE_NAME="notetastic-service"
APP_NAME="notetastic-app"

dotnet publish -c Release NotetasticApi
cp Dockerfile NotetasticApi/bin/Release/netcoreapp2.1/publish
docker build -t $IMAGE_NAME NotetasticApi/bin/Release/netcoreapp2.1/publish
docker tag $IMAGE_NAME registry.heroku.com/$APP_NAME/web
docker push registry.heroku.com/$APP_NAME/web
heroku container:release web -a $APP_NAME
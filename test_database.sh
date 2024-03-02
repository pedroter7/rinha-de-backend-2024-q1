#!/bin/bash
DOCKER_DATABASE_TEST_IMAGE="pedroter7/rinha2024q1-db-${RANDOM}:latest"
DOCKERFILE='DatabaseTest.Dockerfile'

echo "--------------"
echo "Running databse tests using .NET test project"
echo "--------------"
echo "Building Docker image ${DOCKER_DATABASE_TEST_IMAGE} from Dockerfile ${DOCKERFILE}"
echo "--------------"
docker build --rm --tag $DOCKER_DATABASE_TEST_IMAGE --file ${DOCKERFILE} .
echo "--------------"
echo "Running tests"
echo "--------------"
dotnet test -e DOCKER_DATABASE_TEST_IMAGE=${DOCKER_DATABASE_TEST_IMAGE} ./src/PedroTer7.Rinha2024Q1.Database.Tests $1
echo "--------------"
echo "Cleaning up"
docker image rm --force ${DOCKER_DATABASE_TEST_IMAGE}
echo "--------------"
echo "Done"

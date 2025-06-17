APP_NAME=protocol_editor

CONTAINER_NAME=protocol_editor

.PHONY: build run_app stop

build:
	@echo "Сборка Docker-образа базы данных в фоновом режиме"
	docker compose --env-file ../.env up -d
	
run_app:
	@echo "Запуск приложения..."
	dotnet build && dotnet run

stop:
	@echo "Остановка контейнера..."
	docker-compose down

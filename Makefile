APP_NAME=protocol_editor

CONTAINER_NAME=protocol_editor

.PHONY: build run stop

build:
	@echo "Сборка Docker-образа"
	docker build -t 
	
run:
	@echo "Запуск контейнера..."
	docker-compose up --build -d

stop:
	@echo "Остановка контейнера..."
	docker-compose down

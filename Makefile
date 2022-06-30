.PHONY: build clean infra

build:
	@docker-compose build

clean:
	@docker-compose down -v

infra:
	@docker-compose up -d zookeeper kafka schemaregistry

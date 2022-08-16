.PHONY: build tests clean infra

build:
	@docker-compose build

tests:
	@docker-compose up --abort-on-container-exit --exit-code-from tests tests

clean:
	@docker-compose down -v

infra:
	@docker-compose up -d zookeeper kafka schemaregistry

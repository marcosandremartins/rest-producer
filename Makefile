.PHONY: build clean

build:
	@docker-compose build

clean:
	@docker-compose down -v

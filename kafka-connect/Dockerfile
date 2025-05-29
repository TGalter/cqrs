FROM confluentinc/cp-kafka-connect:latest

# Elasticsearch Sink Connector
RUN confluent-hub install --no-prompt confluentinc/kafka-connect-elasticsearch:latest

# Debezium PostgreSQL Source Connector
RUN confluent-hub install --no-prompt debezium/debezium-connector-postgresql:latest

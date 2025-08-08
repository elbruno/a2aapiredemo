# NLWeb Configuration and Data Storage

## nlweb-data/
This directory contains persistent data for the NLWeb container:
- Vector database storage
- Indexed content
- Search indices
- Session data

## nlweb-config/
This directory contains configuration files for NLWeb:
- config.json: Main configuration file
- AI backend settings
- Vector database configuration
- Crawling and indexing parameters

## Data Loading
To load website data into NLWeb, use the following command:
```bash
docker exec -it <container_id> python -m data_loading.db_load <url> <name>
```

For the eShopLite demo:
```bash
docker exec -it <nlweb_container_id> python -m data_loading.db_load https://store eShopLite
```

This will crawl and index the Store website content for natural language search.
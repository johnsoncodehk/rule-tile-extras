# rule-tile-extras

### Setup

The following line needs to be added to your Packages/manifest.json file in your Unity Project under the dependencies section:

```json
"com.johnsoncodehk.rule-tile-extras": "https://github.com/johnsoncodehk/rule-tile-extras.git"
```

### Tilemap

##### Tiles

- **Edge Tile**: The Edge Tile is used to define the map boundary. It is only visible in the editor and will be hidden automatically in play mode.
- **Door Tile**: Door Tile can control the open or closed state of the tile, using GridInformation to store the state.

##### Rule Tiles

- **Sibling Rule Tile**: Sibling Rule Tile can be matched with other Sibling Rule Tile of the same layer.

# rule-tile-extras

### Setup

The following line needs to be added to your Packages/manifest.json file in your Unity Project under the dependencies section:

```json
"com.johnsoncodehk.rule-tile-extras": "https://github.com/johnsoncodehk/rule-tile-extras.git"
```

### Tiles

- **Editor Only Rule Tile**: Editor Only Rule Tile is only visible in the editor and will be hidden automatically in play mode. It can used to define the map edge.
- **Door Rule Tile**: Door Rule Tile can control the open or closed state of the tile, using GridInformation to store the state.
- **Sibling Rule Tile**: Sibling Rule Tile can be matched with other Sibling Rule Tile of the same layer.
- **Wall Rule Tile**: Wall Rule Tile is used to surround any other tiles. [(Sample)](https://user-images.githubusercontent.com/16279759/67807460-cc0ba400-facf-11e9-9a19-03c1843e91e8.png)

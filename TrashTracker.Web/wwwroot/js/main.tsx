import Map from '../lib/openlayers/Map.js';
import OSM from '../lib/openlayers/source/OSM.js';
import TileLayer from '../lib/openlayers/layer/Tile.js';
import View from '../lib/openlayers/View.js';

const map = new Map({
    layers: [
        new TileLayer({
            source: new OSM(),
        }),
    ],
    target: 'map',
    view: new View({
        center: [0, 0],
        zoom: 2,
    }),
});

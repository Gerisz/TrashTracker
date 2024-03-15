"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Map_js_1 = require("../lib/openlayers/Map.js");
var OSM_js_1 = require("../lib/openlayers/source/OSM.js");
var Tile_js_1 = require("../lib/openlayers/layer/Tile.js");
var View_js_1 = require("../lib/openlayers/View.js");
var map = new Map_js_1.default({
    layers: [
        new Tile_js_1.default({
            source: new OSM_js_1.default(),
        }),
    ],
    target: 'map',
    view: new View_js_1.default({
        center: [0, 0],
        zoom: 2,
    }),
});
//# sourceMappingURL=main.js.map
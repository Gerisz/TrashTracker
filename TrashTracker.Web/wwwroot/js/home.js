const defaultCenter = ol.proj.transform([19.0402635, 47.4978867], 'EPSG:4326', 'EPSG:3857');

var map = new ol.Map({
    target: 'map',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        }),
        new ol.layer.Vector({
            source: new ol.source.Vector({
                format: new ol.format.GeoJSON(),
                url: './OnMap'
            })
        })
    ],
    view: new ol.View({
        center: defaultCenter,
        zoom: 7,
    }),
});
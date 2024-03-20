const defaultCenter = ol.proj.transform([19.503736, 47.180086], 'EPSG:4326', 'EPSG:3857');
const statusFills = {
    Cleaned: new ol.style.Fill({
        color: '#0000FF'
    }),
    Less: new ol.style.Fill({
        color: '#00FF00'
    }),
    More: new ol.style.Fill({
        color: '#FF0000'
    }),
    StillHere: new ol.style.Fill({
        color: '#FFFF00'
    })
};

var map = new ol.Map({
    controls: ol.control.defaults.defaults()
        .extend([new ol.control.FullScreen()]),
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        }),
        new ol.layer.Vector({
            source: new ol.source.Vector({
                format: new ol.format.GeoJSON(),
                url: './OnMap'
            }),
            style: new ol.style.Style({
                image: new ol.style.Circle({
                    fill: new ol.style.Fill({
                        color: 'rgba(255,255,255,0.5)'
                    }),
                    stroke: new ol.style.Stroke({
                        color: '#00FFFF',
                        width: 1.25,
                    }),
                    radius: 5,
                }),
                fill: new ol.style.Fill({
                    color: 'rgba(255,255,255,0.5)'
                }),
                stroke: new ol.style.Stroke({
                    color: '#00FFFF',
                    width: 1.25,
                }),
            })
        })
    ],
    target: 'map',
    view: new ol.View({
        center: defaultCenter,
        zoom: 7,
    }),
});
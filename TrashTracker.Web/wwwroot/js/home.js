const defaultCenter = ol.proj.transform([19.503736, 47.180086], 'EPSG:4326', 'EPSG:3857');
const fillStatuses = {
    Cleaned: new ol.style.Fill({
        color: '#198754'
    }),
    Less: new ol.style.Fill({
        color: '#FFC107'
    }),
    More: new ol.style.Fill({
        color: '#DC3545'
    }),
    StillHere: new ol.style.Fill({
        color: '#FD7E14'
    })
};
const sizeRadiuses = { Bag: 6, Wheelbarrow: 8, Car: 10 };
const styleCache = {};

var trashTrackerMap = new ol.Map({
    controls: ol.control.defaults.defaults()
        .extend([new ol.control.FullScreen()]),
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        }),
        new ol.layer.Vector({
            source: new ol.source.Cluster({
                source: new ol.source.Vector({
                    format: new ol.format.GeoJSON(),
                    url: './OnMap'
                })
            }),
            style: function (feature) {
                const size = feature.get('features').length;
                let style = styleCache[size];
                if (size > 1) {
                    if (!style) {
                        style = new ol.style.Style({
                            image: new ol.style.Circle({
                                fill: new ol.style.Fill({
                                    color: '#888888'
                                }),
                                stroke: new ol.style.Stroke({
                                    color: '#000000',
                                    width: 1
                                }),
                                radius: 10
                            }),
                            text: new ol.style.Text({
                                text: size != 1 ? size.toString() : "",
                                fill: new ol.style.Fill({
                                    color: '#FFFFFF',
                                }),
                            })
                        });
                        styleCache[size] = style;
                    }
                    return style;
                }
                const point = feature.get('features')[0];
                return new ol.style.Style({
                    image: new ol.style.Circle({
                        fill: fillStatuses[point.get('status')],
                        stroke: new ol.style.Stroke({
                            color: '#000000',
                            width: 1
                        }),
                        radius: sizeRadiuses[point.get('size')]
                    }),
                    text: new ol.style.Text({
                        text: size != 1 ? size.toString() : "",
                        fill: new ol.style.Fill({
                            color: '#FFFFFF',
                        }),
                    })
                });
            }
        })
    ],
    target: 'map',
    view: new ol.View({
        center: defaultCenter,
        zoom: 7,
    }),
});
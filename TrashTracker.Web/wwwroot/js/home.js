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
const overlay = new ol.Overlay({
    element: document.getElementById('popup'),
    autoPan: {
        animation: {
            duration: 250
        }
    }
});
const sizeRadiuses = { Bag: 6, Wheelbarrow: 8, Car: 10 };
const styleCache = {};

/*closer.onclick = function () {
    overlay.setPosition(undefined);
    closer.blur();
    return false;
};*/

class RotateNorthControl extends ol.control.Control {
    /**
     * @param {Object} [opt_options] Control options.
     */
    constructor(opt_options) {
        const options = opt_options || {};

        const button = document.createElement('button');
        button.innerHTML = 'N';

        const element = document.createElement('div');
        element.className = 'rotate-north ol-unselectable ol-control';
        element.appendChild(button);

        super({
            element: element,
            target: options.target,
        });

        button.addEventListener('click', this.handleRotateNorth.bind(this), false);
    }

    handleRotateNorth() {
        this.getMap().getView().setRotation(0);
    }
}

let map = new ol.Map({
    controls: ol.control.defaults.defaults()
        .extend([
            new ol.control.FullScreen(),
            new RotateNorthControl()
        ]),
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
    overlays: [overlay],
    target: 'map',
    view: new ol.View({
        center: defaultCenter,
        zoom: 7,
    }),
});

const element = overlay.getElement();
map.on('click', function (evt) {
    let features;
    try {
        features = map.getFeaturesAtPixel(evt.pixel)[0].get('features');
    } catch { return; }
    if (features.length == 1) {
        const feature = features[0];
        const coordinate = feature.getGeometry().getCoordinates();
        const hdms = ol.coordinate.toStringHDMS(ol.proj.toLonLat(coordinate));
        const id = feature.get('id');
        overlay.setPosition(coordinate);
        let popover = bootstrap.Popover.getInstance(element);
        if (popover)
            popover.dispose();
        popover = new bootstrap.Popover(element, {
            animation: false,
            container: element,
            content: `<p>
                          ${hdms}
                      </p>
                      <a class="btn btn-info" data-bs-title="Részletek" data-bs-toggle="tooltip"
                              href="/Trashes/Details/${id}">
                          <i class="fa-solid fa-circle-info" aria-hidden="true"></i>
                          <span>Részletek</span>
                      </a>`,
            html: true,
            placement: 'top',
            title: `${id}. szemétpont`,
        });
        popover.show();
    }
});
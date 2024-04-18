const searchParams = new URLSearchParams(window.location.search);
const defaultCenter = ol.proj.transform([
    searchParams.get("lat") ?? 19.503736,
    searchParams.get("lon") ?? 47.180086
], 'EPSG:4326', 'EPSG:3857');
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

let points = new ol.source.Vector({
    format: new ol.format.GeoJSON(),
    url: './OnMap'
});

let map = new ol.Map({
    controls: ol.control.defaults.defaults()
        .extend([
            new ol.control.FullScreen()
        ]),
    interactions: ol.interaction.defaults.defaults()
        .extend([
            new ol.interaction.DragRotateAndZoom()
        ]),
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        }),
        new ol.layer.Vector({
            source: new ol.source.Cluster({
                distance: 50,
                minDistance: 10,
                source: points
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
        zoom: (searchParams.get("lat"), searchParams.get("lon")) != (null, null) ? 13 : 8
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
        overlay.setPosition(coordinate);
        const id = feature.get('id');
        $.getJSON(`./OnMapDetails/${id}`, function (data) {
            let popover = bootstrap.Popover.getInstance(element);
            if (popover)
                popover.dispose();
            popover = new bootstrap.Popover(element, {
                container: element,
                content: popoverContent(data),
                html: true,
                placement: 'top',
                title: `${id}. szemétpont`,
            });
            popover.show();
        });
    }
});

function filter() {
    let accessibilities = Array.from(document.getElementsByClassName('accessibility'))
        .filter(e => e.checked)
        .map(e => e.id);
    let sizes = Array.from(document.getElementsByClassName('size'))
        .filter(e => e.checked)
        .map(e => e.id);
    let statuses = Array.from(document.getElementsByClassName('status'))
        .filter(e => e.checked)
        .map(e => e.id);
    let types = Array.from(document.getElementsByClassName('type'))
        .filter(e => e.checked)
        .map(e => e.id);
    map.getAllLayers()[1].setSource(new ol.source.Cluster({
        source: new ol.source.Vector({
            features: points.getFeatures()
                .filter(p => accessibilities.some(a => p.get('accessibilities').includes(a)))
                .filter(p => sizes.includes(p.get('size')))
                .filter(p => statuses.includes(p.get('status')))
                .filter(p => types.some(t => p.get('types').includes(t)))
        })
    }));
}

function popoverContent(data) {
    return `
        <div class="carousel slide">
            <div class="carousel-inner">
                ${data.images ? `
                    <div class="carousel-item active">
                        <img src="${data.images.shift()}" class="d-block w-100">
                    </div>` : ``}
                ${data.images.map(i => `
                    <div class="carousel-item">
                        <img src="${i}" class="d-block w-100">
                    </div>`
                )}
            </div>
            <button class="carousel-control-prev" type="button" data-bs-slide="prev">
                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Previous</span>
            </button>
            <button class="carousel-control-next" type="button" data-bs-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Next</span>
            </button>
        </div>
        <a class="btn btn-info" data-bs-title="Részletek" data-bs-toggle="tooltip"
                   href="/Trashes/Details/${data.id}">
               <i class="fa-solid fa-circle-info" aria-hidden="true"></i>
                   <span>Részletek</span>
        </a>`;
}
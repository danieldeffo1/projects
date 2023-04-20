import QtQuick 2.9
import QtQuick.Window 2.2
import Cars 1.0
import Lights 1.0

Window {
    id: root
    visible: true
    width: road.width
    height: road.height
    maximumHeight: height
    minimumHeight: height
    maximumWidth: width
    minimumWidth: width

    Image {
        id: road
        source: "pictures/road.jpg"

        Text{
            anchors.left: parent.left
            anchors.bottom: parent.bottom
            anchors.margins: 10
            text: "Press C to\nrestart animation"
            font.pointSize: 16
        }

        Light {
            rotation: 90
            x: 100
            y: 200
            lights: _lights.lights[0]
        }

        Light {
            x: 75
            y: 35
            lights: _lights.lights[1]
        }

        Light {
            rotation: 90
            x: 280
            y: 60
            lights: _lights.lights[2]
        }

        Light {
            x: 220
            y: 200
            lights: _lights.lights[3]
        }

        Car1 {
            text: game.car1Turns
            mirror: game.car1Reversed
            id: car1
            x: game.car1.x
            y: game.car1.y
        }

        Car2 {
            text: game.car2Turns
            mirror: game.car2Reversed
            id: car2
            x: game.car2.x
            y: game.car2.y
        }

        Cars_qml {
            id: game
            lights {
                id: _lights
            }
        }
        KeyCatcher{
            _gameId: game
        }
    }
}

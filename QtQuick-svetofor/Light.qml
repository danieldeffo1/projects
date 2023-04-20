import QtQuick 2.0

Item {
    id: root
    width: _img.width
    height: _img.height
    property int lights
    property int rotation: 0

    Image { 
        transform: Rotation{
            angle: root.rotation
        }
        x: root.x
        y: root.y
        id: _img
        source: "pictures/Light.png"

        Rectangle {
            x: 13
            y: 90
            width: 28
            height: 28
            radius: 28
            color: "#66FF00"
            visible: root.lights===0
        }

        Rectangle {
            x: 13
            y: 52
            width: 28
            height: 28
            radius: 28
            color: "#FFB300"
            visible: root.lights===1
        }

        Rectangle {
            x: 13
            y: 15
            width: 28
            height: 28
            radius: 28
            color: "#FF2400"
            visible: root.lights===2
        }
    }
}

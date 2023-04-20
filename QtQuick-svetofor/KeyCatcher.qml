import QtQuick 2.0
import Cars 1.0

Item {
    focus: true
    property Cars_qml _gameId
    Keys.onPressed: {
        _gameId.keyCatch(event.text)
    }
}

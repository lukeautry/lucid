import './map-editor.scss';
import { buildMap, ICell } from './build-map';
import { Area, Room } from '../../../../../api/api';
import * as React from 'react';
import Paper from 'material-ui/Paper';
import RaisedButton from 'material-ui/RaisedButton';
import { Direction } from '../view-area';

export interface IMapEditorProps {
  area: Area;
  rooms: Room[];
  addNewRoom: () => void;
  addConnectingRoom: (callback: (newRoomId: number) => Room) => any;
  updateRoom: (room: Room) => void;
  editNewRoom: (room: Room) => any;
}

const directionAugmenters: { [direction: string]: (room: Room, newRoomId: number) => any } = {
  'east': (r, newRoomId) => r.eastRoomId = newRoomId,
  'south': (r, newRoomId) => r.southRoomId = newRoomId,
  'west': (r, newRoomId) => r.westRoomId = newRoomId,
  'north': (r, newRoomId) => r.northRoomId = newRoomId,
  'up': (r, newRoomId) => r.upRoomId = newRoomId,
  'down': (r, newRoomId) => r.downRoomId = newRoomId,
};

export const MapEditor = (props: IMapEditorProps) => {
  const map = !!(props.rooms && props.rooms.length) && buildMap(props.rooms);

  const adjacentCellMap: { [direction: string]: (cell: ICell) => ICell | undefined } = {
    'east': cell => cell.parent.cells[cell.index + 1],
    'west': cell => cell.parent.cells[cell.index - 1],
    'south': cell => {
      const southRow = cell.parent.parent.rows[cell.parent.index + 1];
      if (!southRow) { return; }

      return southRow.cells[cell.index];
    },
    'north': cell => {
      const northRow = cell.parent.parent.rows[cell.parent.index - 1];
      if (!northRow) { return; }

      return northRow.cells[cell.index];
    }
  };

  const directionHandler = (cell: ICell, direction: Direction) => () => {
    const adjacentCell = adjacentCellMap[direction](cell);
    if (adjacentCell && adjacentCell.room) {
      const clone = Object.assign({}, cell.room) as Room;
      directionAugmenters[direction](clone, adjacentCell.room.id);
      props.updateRoom(clone);
      return;
    }

    props.addConnectingRoom(newRoomId => {
      const clone = Object.assign({}, cell.room) as Room;
      directionAugmenters[direction](clone, newRoomId);

      return clone;
    });
  };

  const editRoomHandler = (room: Room) => () => props.editNewRoom(room);

  return (
    <div className='map-editor'>
      {
        map ?
          map.levels.map(level => {
            return level.rows.map((row, rowIndex) => (
              <div className='map-row' key={rowIndex}>
                {row.cells.map((cell, cellIndex) => (
                  <div className='map-cell' key={cellIndex}>
                    {
                      cell.room &&
                      <Paper zDepth={0} rounded={true} style={{ width: '100%', height: '100%', position: 'relative', display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                        <div className='title'>{cell.room.name}</div>
                        <i className={`material-icons arrow east ${cell.hasEastExit ? 'active' : ''}`} onClick={directionHandler(cell, 'east')}>arrow_forward</i>
                        <i className={`material-icons arrow west ${cell.hasWestExit ? 'active' : ''}`} onClick={directionHandler(cell, 'west')}>arrow_back</i>
                        <i className={`material-icons arrow north ${cell.hasNorthExit ? 'active' : ''}`} onClick={directionHandler(cell, 'north')}>arrow_upward</i>
                        <i className={`material-icons arrow south ${cell.hasSouthExit ? 'active' : ''}`} onClick={directionHandler(cell, 'south')}>arrow_downward</i>
                        <i className='edit material-icons btn' title='Edit' onClick={editRoomHandler(cell.room)}>mode_edit</i>
                      </Paper>
                    }
                  </div>
                ))}
              </div>
            ));
          })
          : <RaisedButton primary={true} className='add-new-room-button' label='Add a new room' onClick={props.addNewRoom} />
      }
    </div>
  );
};

import { Room } from '../../../../../api/api';
import { Direction } from '../view-area';

export interface IMap {
  levels: ILevel[];
}

export interface ILevel {
  index: number;
  rows: IRow[];
}

export interface IRow {
  index: number;
  cells: ICell[];
  parent: ILevel;
}

export interface ICell {
  index: number;
  room?: Room;
  parent: IRow;
  hasEastExit: boolean;
  hasWestExit: boolean;
  hasNorthExit: boolean;
  hasSouthExit: boolean;
}

export const buildMap = (rooms: Room[]): IMap => {
  const rows = new Array<IRow>();
  // For now, just one level
  const levels: ILevel[] = [{ index: 0, rows }];

  const placedCells: { [id: number]: ICell } = {};
  const roomMap: { [id: number]: Room } = {};
  const processingMap: { [id: number]: boolean } = {};
  const linkMap: { [id: number]: { [direction: string]: number[] } } = {};
  const registerLink = (linkedId: number, linkingId: number, direction: Direction) => {
    linkMap[linkedId] = linkMap[linkedId] || {};
    linkMap[linkedId][direction] = linkMap[linkedId][direction] || new Array<number>();
    linkMap[linkedId][direction].push(linkingId);
  };

  rooms.forEach(room => {
    roomMap[room.id] = room;
    if (room.eastRoomId) { registerLink(room.eastRoomId, room.id, 'east'); }
    if (room.southRoomId) { registerLink(room.southRoomId, room.id, 'south'); }
    if (room.westRoomId) { registerLink(room.westRoomId, room.id, 'west'); }
    if (room.northRoomId) { registerLink(room.northRoomId, room.id, 'north'); }
    if (room.upRoomId) { registerLink(room.upRoomId, room.id, 'up'); }
    if (room.downRoomId) { registerLink(room.downRoomId, room.id, 'down'); }
  });

  const findCellByCoordinates = (x: number, y: number, z: number) => {
    const level = levels.find(l => l.index === z);
    if (!level) { return undefined; }

    const row = level.rows.find(r => r.index === y);
    if (!row) { return undefined; }

    return row.cells.find(c => c.index === x);
  };

  const buildCell = (index: number, parent: IRow, room?: Room) => {
    return {
      index,
      parent,
      room,
      hasEastExit: !!(room && room.eastRoomId),
      hasSouthExit: !!(room && room.southRoomId),
      hasWestExit: !!(room && room.westRoomId),
      hasNorthExit: !!(room && room.northRoomId)
    };
  };

  const processLinks = (roomId: number, direction: Direction, newCellFn: (cell: ICell) => ICell) => {
    const link = linkMap[roomId];
    if (!link) { return; }

    const directionLinks = link[direction];
    if (!directionLinks) { return; }
    if (directionLinks.length > 1) { throw new Error('Looks like multiple rooms link to this room from the same direction?'); }

    const directionLink = directionLinks[0];

    let cell = placedCells[directionLink];
    if (cell) {
      placedCells[roomId] = newCellFn(cell);
      return placedCells[roomId];
    }

    if (processingMap[directionLink]) { return; }

    processingMap[roomId] = true;
    /* tslint:disable-next-line */
    const placedCell = placeRoom(roomMap[directionLink]);
    if (!placedCell) { return; }
    cell = placedCell;
    placedCells[roomId] = newCellFn(cell);
    return placedCells[roomId];
  };

  const processEast = (room: Room) => {
    return processLinks(room.id, 'east', cell => {
      const existingCell = findCellByCoordinates(cell.index + 1, cell.parent.index, 0);
      if (existingCell) { throw new Error('There\'s already a cell here.'); }

      const newCell = buildCell(cell.index + 1, cell.parent, room);
      cell.parent.cells.push(newCell);
      placedCells[room.id] = newCell;

      return newCell;
    });
  };

  const processWest = (room: Room) => {
    return processLinks(room.id, 'west', cell => {
      const existingCell = findCellByCoordinates(cell.index - 1, cell.parent.index, 0);
      if (existingCell) { throw new Error('There\'s already a cell here.'); }

      const newCell = buildCell(cell.index - 1, cell.parent, room);
      cell.parent.cells.push(newCell);
      placedCells[room.id] = newCell;

      return newCell;
    });
  };

  const processNorth = (room: Room) => {
    return processLinks(room.id, 'north', cell => {
      const existingCell = findCellByCoordinates(cell.index, cell.parent.index - 1, 0);
      if (existingCell) { throw new Error('There\'s already a cell here.'); }

      let northRow = rows.find(row => row.index === cell.parent.index - 1);
      if (!northRow) {
        northRow = { index: cell.parent.index - 1, cells: new Array<ICell>(), parent: cell.parent.parent };
        rows.push(northRow);
      }

      const newCell = buildCell(cell.index, northRow, room);
      northRow.cells.push(newCell);
      placedCells[room.id] = newCell;

      return newCell;
    });
  };

  const processSouth = (room: Room) => {
    return processLinks(room.id, 'south', cell => {
      const existingCell = findCellByCoordinates(cell.index, cell.parent.index + 1, 0);
      if (existingCell) {
        throw new Error('There\'s already a cell here.');
      }

      let southRow = rows.find(row => row.index === cell.parent.index + 1);
      if (!southRow) {
        southRow = { index: cell.parent.index + 1, cells: new Array<ICell>(), parent: cell.parent.parent };
        rows.push(southRow);
      }

      const newCell = buildCell(cell.index, southRow, room);
      southRow.cells.push(newCell);
      placedCells[room.id] = newCell;

      return newCell;
    });
  };

  const placeRoom = (room: Room) => {
    if (placedCells[room.id]) { return; }

    const linkedCell = processEast(room) || processWest(room) || processNorth(room) || processSouth(room);
    if (linkedCell) { return linkedCell; }

    // Evidently this is the first room
    if (!rows[0]) {
      const newRow: IRow = { parent: levels[0], index: 0, cells: [] };
      const newCell = buildCell(0, newRow, room);

      newRow.cells.push(newCell);
      rows.push(newRow);
      placedCells[room.id] = newCell;
      return newCell;
    }

    /**
     * The self balancing map - eventually make this better so that cell 'patches' are distributed evenly
     */
    let maxCellIndex = 0;
    rows[0].cells.forEach(cell => {
      if (cell.index > maxCellIndex) {
        maxCellIndex = cell.index;
      }
    });

    const cell = buildCell(maxCellIndex + 2, rows[0], room);
    rows[0].cells.push(cell);
    placedCells[room.id] = cell;
    return cell;
  };

  rooms.forEach(room => placeRoom(room));

  const sortedLevels = levels.sort((a, b) => a.index - b.index);
  sortedLevels.forEach(level => {
    let minCellIndex = 0;
    let maxCellIndex = 0;
    let minRowIndex = 0;

    level.rows = level.rows.sort((a, b) => a.index - b.index);
    level.rows.forEach(row => {
      row.cells = row.cells.sort((a, b) => a.index - b.index);

      const maxIndex = row.cells[row.cells.length - 1].index;
      if (maxIndex > maxCellIndex) { maxCellIndex = maxIndex; }

      const minIndex = row.cells[0].index;
      if (minIndex < minCellIndex) { minCellIndex = minIndex; }

      const rowIndex = row.index;
      if (rowIndex < minCellIndex) { minRowIndex = rowIndex; }
    });

    level.rows.forEach(row => {
      row.index = row.index - minRowIndex;

      const normalizedCellCount = maxCellIndex - minCellIndex;

      // [-1, 0, 1] --> [0, 1, 2]
      const normalizedCells = row.cells.map(cell => { cell.index = cell.index - minCellIndex; return cell; });
      const cells = new Array<ICell>();

      for (let i = 0; i <= normalizedCellCount; i++) {
        const existingCell = normalizedCells.find(c => c.index === i);
        if (existingCell) {
          cells.push(existingCell);
          continue;
        }

        cells.push(buildCell(i, row));
      }
      row.cells = cells;
    });
  });

  return { levels: sortedLevels };
};

extends Object
class_name PieceBoard

var width : int
var height : int
var board : Array # TODO CREATE A MATRIX TYPE

func _init(size : Vector2i):
	self.width = size.x
	self.height = size.y
	self.board = Array()
	for i in range(self.height):
		var line = Array()
		for j in range(self.width):
			line.insert(0, Vector2i(0, 0))
		self.board.insert(0, line)

func _to_string():
	var t = ""
	for line in board:
		var s = ""
		for cell in line:
			s += str(cell) + ", "
		t += s.substr(0, len(s) - 2) + "\n"
	return t

func CanPlaceCell(pos : Vector2i) -> bool:
	if 0 > pos.y || pos.y >= self.height:
		return false
	if 0 > pos.x || pos.x >= self.width:
		return false
	return self.board[pos.y][pos.x].length_squared() == 0

func CanPlace(pos : Vector2i, piece : Piece) -> bool:
	for cell in piece.cells:
		if !self.CanPlaceCell(pos + cell):
			return false
	return true

func Place(pos : Vector2i, piece : Piece) -> bool:
	if !self.CanPlace(pos, piece):
		return false
	
	for cell in piece.cells:
		var p = pos + cell
		self.board[p.y][p.x] = piece.id
	
	return true

func isEmptyCell(cell : Vector2i) -> bool:
	return cell.length_squared() == 0

func isLineEmpty(line : Array) -> bool:
	return line.all(isEmptyCell)
	
func isLineBreakable(line : Array) -> bool:
	return !line.any(isEmptyCell)

func MoveLineDown(lineIndex : int, amount : int):
	for x in self.width:
		self.board[lineIndex + amount][x] = self.board[lineIndex][x]
		self.board[lineIndex][x] = Vector2i(0, 0)

func MoveAllLinesDown(startIndex : int, amount : int):
	for y in range(startIndex, -1, - 1):
		MoveLineDown(y, amount)

func ShiftDown():
	var emptyLines = 0
	for y in range(self.height - 1, -1, -1):
		# Count how many lines to clear
		if isLineEmpty(self.board[y]): 
			emptyLines += 1
			continue
		
		# If no empty lines were found yet, keep going
		if emptyLines == 0: continue
		
		# Move all lines down
		MoveAllLinesDown(y, emptyLines)
		return

func BreakLine(line : Array) -> bool:
	if !isLineBreakable(line): return false
	
	# Clear line
	for i in self.width:
		line[i] = Vector2i(0, 0)
	return true

func BreakLines() -> int:
	var res = 0
	for line in board:
		if BreakLine(line): res += 1
	if res > 0:
		ShiftDown()
	return res

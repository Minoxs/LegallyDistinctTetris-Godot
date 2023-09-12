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

func CanPlaceCell(pos : Vector2i) -> bool:
	if 0 > pos.y || pos.y >= self.height:
		return false
	if 0 > pos.x || pos.x >= self.width:
		return false
	return true

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

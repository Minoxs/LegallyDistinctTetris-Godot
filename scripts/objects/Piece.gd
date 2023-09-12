extends Object
class_name Piece

var id
var cells : Array[Vector2i]

func _init(id, cells : Array[Vector2i]):
	self.id = id
	self.cells = cells.duplicate(true)

func _to_string():
	return str(self.id) + " | " + str(self.cells)

func duplicate():
	return Piece.new(self.id, self.cells.duplicate(true))

func Rotate():
	var res = self.duplicate()
	for i in len(self.cells):
		var cell = self.cells[i]
		res.cells[i].x = -cell.y
		res.cells[i].y = -cell.x
	return res

extends TileMap

# Drawing stuff
@onready var PIECE_I = self.tile_set.get_pattern(0)
@onready var PIECE_Z = self.tile_set.get_pattern(1)
@onready var PIECE_T = self.tile_set.get_pattern(2)
@onready var PIECE_J = self.tile_set.get_pattern(3)
@onready var PIECE_S = self.tile_set.get_pattern(4)
@onready var PIECE_L = self.tile_set.get_pattern(5)
@onready var PIECE_O = self.tile_set.get_pattern(6)
@onready var SQUARE_UNIT = self.cell_quadrant_size

const PIECE_WHITE = Vector2(0, 0)
const PIECE_BLACK = Vector2(1, 0)

func PieceFromPattern(pattern : TileMapPattern) -> Piece:
	var cells = pattern.get_used_cells()
	return Piece.new(pattern.get_cell_atlas_coords(cells[0]), cells)

# Game settings
var X_SIZE : int = 0 
var Y_SIZE : int = 0
var SPAWN        = Vector2(0, 0)
const TICK_RATE  = 1

# Runtime data
var TICK_STEP = 0
var REAL_BOARD : PieceBoard = null

var PIECE_POS = Vector2(0, 0)
var PIECE_TYP : TileMapPattern = null
var PIECE_CUR : Piece = null

func createBoard(x_size : int, y_size : int):
	X_SIZE = x_size
	Y_SIZE = y_size
	SPAWN  = Vector2(floor(X_SIZE / 2), -2)
	
	REAL_BOARD = PieceBoard.new(Vector2i(X_SIZE, Y_SIZE))
	self.PIECE_TYP = PIECE_I
	self.PIECE_CUR = PieceFromPattern(PIECE_I)
	
	for y in range(Y_SIZE):
		self.set_cell(0, Vector2(-1    , y), 0, PIECE_BLACK)
		self.set_cell(0, Vector2(X_SIZE, y), 0, PIECE_BLACK)
	
	for x in range(-1, X_SIZE + 1):
		self.set_cell(0, Vector2(x, Y_SIZE), 0, PIECE_BLACK)

func tick(delta):
	render()
	TICK_STEP += delta
	if TICK_STEP >= TICK_RATE:
		TICK_STEP -= TICK_RATE
		
		self.clear_layer(1)
		if self.REAL_BOARD.CanPlace(PIECE_POS + Vector2(0, 1), PIECE_CUR):
			print("piece moved")
			PIECE_POS += Vector2(0, 1)
			self.set_pattern(1, PIECE_POS, PIECE_TYP)
		else:
			print("piece set")
			self.REAL_BOARD.Place(PIECE_POS, PIECE_CUR)

func render():
	for y in REAL_BOARD.height:
		for x in REAL_BOARD.width:
			var pos : Vector2i = REAL_BOARD.board[y][x]
			if pos.length() > 0:
				self.set_cell(0, Vector2(x, y), 0, pos)

# Called when the node enters the scene tree for the first time.
func _ready():
	createBoard(10, 20)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var window = get_viewport_rect().size
	self.position.x = (window.x - (X_SIZE + 0) * SQUARE_UNIT * self.scale.x) / 2
	self.position.y = (window.y - (Y_SIZE + 1) * SQUARE_UNIT * self.scale.y)
	tick(delta)

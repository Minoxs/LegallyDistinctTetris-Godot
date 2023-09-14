extends TileMap

# Drawing stuff
const PIECE_WHITE = Vector2(0, 0)
const PIECE_BLACK = Vector2(1, 0)
@onready var SQUARE_UNIT = self.cell_quadrant_size

# Game settings
var X_SIZE : int = 0 
var Y_SIZE : int = 0
var SPAWN        = Vector2(0, 0)
const TICK_RATE  = 0.25

# Runtime data
var SCORE : int = 0
var TICK_STEP = 0
var REAL_BOARD : PieceBoard = null

var PIECE_POS = Vector2i(0, 0)
var PIECE_CUR : Piece = null

func PieceFromPattern(pattern : TileMapPattern) -> Piece:
	var cells = pattern.get_used_cells()
	return Piece.new().Init(pattern.get_cell_atlas_coords(cells[0]), cells)

func GenerateNewPiece() -> bool:
	var dice  = randi_range(0, self.tile_set.get_patterns_count() - 1)
	PIECE_POS = Vector2i(SPAWN)
	var patte = self.tile_set.get_pattern(dice)
	PIECE_CUR = PieceFromPattern(patte)
	return self.REAL_BOARD.CanPlace(SPAWN, PIECE_CUR)

func RotatePiece():
	var r = PIECE_CUR.Rotate()
	if self.REAL_BOARD.CanPlace(PIECE_POS, r):
		PIECE_CUR = r

func MovePiece(mov : Vector2i) -> bool:
	if !self.REAL_BOARD.CanPlace(PIECE_POS + mov, PIECE_CUR):
		return false
	PIECE_POS += mov
	return true

func BreakBlocks():
	SCORE += 25 * self.REAL_BOARD.BreakLines() ** 2
	print(SCORE)

func GameOver():
	print("Game Over!")
	set_process_input(false)
	set_process(false)

func createBoard(x_size : int, y_size : int):
	X_SIZE = x_size
	Y_SIZE = y_size
	SPAWN  = Vector2(floor(X_SIZE / 2), 0)
	
	REAL_BOARD = PieceBoard.new(Vector2i(X_SIZE, Y_SIZE))
	GenerateNewPiece()
	
	for y in range(Y_SIZE):
		self.set_cell(2, Vector2(-1    , y), 0, PIECE_BLACK)
		self.set_cell(2, Vector2(X_SIZE, y), 0, PIECE_BLACK)
	
	for x in range(-1, X_SIZE + 1):
		self.set_cell(2, Vector2(x, Y_SIZE), 0, PIECE_BLACK)

func tick(delta):
	TICK_STEP += delta
	if TICK_STEP >= TICK_RATE:
		TICK_STEP -= TICK_RATE
		if !MovePiece(Vector2i.DOWN):
			self.REAL_BOARD.Place(PIECE_POS, PIECE_CUR)
			BreakBlocks()
			if !GenerateNewPiece(): GameOver()

func render(fast : bool = false):
	# Render current piece in layer 1
	self.clear_layer(1)
	for cell in PIECE_CUR.Cells:
		self.set_cell(1, cell + PIECE_POS, 0, PIECE_CUR.Value)
	
	if fast: return
	
	# Render board in layer 0
	self.clear_layer(0)
	for y in REAL_BOARD.height:
		for x in REAL_BOARD.width:
			var pos : Vector2i = REAL_BOARD.board[y][x]
			if pos.length_squared() > 0:
				self.set_cell(0, Vector2(x, y), 0, pos)

# Handle input
func _input(event):
	if Input.is_action_pressed("ui_up"):
		RotatePiece()
	if Input.is_action_pressed("ui_right"):
		MovePiece(Vector2i.RIGHT)
	if Input.is_action_pressed("ui_left"):
		MovePiece(Vector2i.LEFT)
	if Input.is_action_pressed("ui_down"):
		tick(max(TICK_RATE - TICK_STEP, 0))
	render(true)

# Called when the node enters the scene tree for the first time.
func _ready():
	createBoard(10, 20)
	render()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var window = get_viewport_rect().size
	self.position.x = (window.x - (X_SIZE + 0) * SQUARE_UNIT * self.scale.x) / 2
	self.position.y = (window.y - (Y_SIZE + 1) * SQUARE_UNIT * self.scale.y)
	tick(delta)
	render()

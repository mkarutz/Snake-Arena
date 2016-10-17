package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Rect;
import in.slyther.math.Vector2;
import slyther.flatbuffers.NetworkObjectStateType;
import slyther.flatbuffers.NetworkSnakeState;

/**
 *
 */
public class Snake implements GameObject {
    private static final float PI = (float) Math.PI;
    public static final int GROWTH_CAP = 40000;
    public static final float MIN_LENGTH = 1.0f;
    public static final float MIN_THICKNESS = 0.2f;
    public static final int MIN_TURBO_SCORE = 100;

    public static final float TAIL_SLACK = 5.0f;
    public static final float GROWTH_RATE = 1.0f / 100.0f;
    public static final float MOVE_SPEED = 1.9f;
    public static final float TURBO_BOOST_FACTOR = 3.0f;
    public static final float TURN_RADIUS_FACTOR = 2.5f;
    public static final int MAX_PARTS = 1000;
    public static final float EAT_DISTANCE_RATIO = 1.0f;
    public static final float BURN_SPEED = 75;

    public static final float MAX_SEGMENT_ANGLE_DEG = 5.0f;
    public static final float MAX_SEGMENT_ANGLE = MAX_SEGMENT_ANGLE_DEG * PI / 180.0f;

    private final int pid;
    private String name;
    private int skin;
    private int score;
    private boolean isTurbo;
    private boolean isDead;

    private final SnakePart[] parts = new SnakePart[MAX_PARTS];
    private int headPointer;
    private int tailPointer;

    private Rect boundingBox = Rect.unit();


    /**
     * Snake constructor.
     * @param pid Player ID.
     * @param name Player name.
     * @param score Player score.
     */
    public Snake(int pid, String name, int score) {
        this.pid = pid;
        this.name = name;
        this.score = score;

        initSnakeParts();
        respawn(Vector2.zero(), getScore());

        setDead(true);
    }


    @Override
    public byte classId() {
        return NetworkObjectStateType.NetworkSnakeState;
    }


    @Override
    public int serialize(FlatBufferBuilder builder) {
        int[] partsOffsets = new int[MAX_PARTS];
        int n = 0;

        for (int i = headPointer; i != tailPointer; i = nextPointer(i)) {
            partsOffsets[n++] = parts[i].serialize(builder);
        }

        //partsOffsets[n++] = parts[headPointer].serialize(builder);

        int vectorOffset = NetworkSnakeState.createPartsVector(builder, partsOffsets, n);
        int nameOffset = builder.createString(name);
        String skinId = Integer.toString(skin);
        int skinOffset = builder.createString(skinId);
        NetworkSnakeState.startNetworkSnakeState(builder);
        NetworkSnakeState.addParts(builder, vectorOffset);
        NetworkSnakeState.addHead(builder, headPointer);
        NetworkSnakeState.addIsDead(builder, isDead);
        NetworkSnakeState.addIsTurbo(builder, isTurbo());
        NetworkSnakeState.addName(builder, nameOffset);
        NetworkSnakeState.addScore(builder, score);
        NetworkSnakeState.addTail(builder, tailPointer);
        NetworkSnakeState.addSkin(builder, skinOffset);

        return NetworkSnakeState.endNetworkSnakeState(builder);
    }


    /**
     * Initializes the parts of a new snake.
     */
    private void initSnakeParts() {
        for (int i = 0; i < MAX_PARTS; i++) {
            parts[i] = new SnakePart(i, Vector2.zero());
        }
    }


    /**
     * Updates the bounding box of this snake.
     */
    public void updateBoundingBox() {
        float x0 = Float.MAX_VALUE;
        float x1 = Float.MIN_VALUE;
        float y0 = Float.MAX_VALUE;
        float y1 = Float.MIN_NORMAL;

        float radius = getThickness() / 2.0f;

        for (int i = headPointer; i != tailPointer; i = nextPointer(i)) {
            x0 = Math.min(x0, parts[i].getPosition().getX() - radius);
            x1 = Math.max(x1, parts[i].getPosition().getX() + radius);
            y0 = Math.min(y0, parts[i].getPosition().getY() - radius);
            y1 = Math.max(y1, parts[i].getPosition().getY() + radius);
        }

        boundingBox.setMinX(x0);
        boundingBox.setMinY(y0);
        boundingBox.setMaxX(x1);
        boundingBox.setMaxY(y1);
    }


    /**
     * Respawns the snake.
     * @param position Starting position.
     * @param startingScore Starting starting score.
     */
    public void respawn(Vector2 position, int startingScore) {
        score = startingScore;
        headPointer = 0;
        tailPointer = 500;
        isDead = false;
        isTurbo = false;

        for (int i = 0; i < MAX_PARTS; i++) {
            parts[i].getPosition().set(position);
        }

        updateBoundingBox();
    }


    public boolean canReachFoodAt(Vector2 point) {
        return Vector2.distance(getHeadPosition(), point) < getEatDistance();
    }


    public boolean isRunInto(Snake other) {
        return other != this && other.intersects(getHeadPosition());
    }


    public boolean intersects(Vector2 point) {
        if (!boundingBox.collidesWith(point)) {
            return false;
        }

        int curr = headPointer;
        int next = nextPointer(headPointer);
        float accLength = 0.0f;

        for ( ; next != tailPointer; curr = next, next = nextPointer(next)) {
            Vector2 currPoint = parts[curr].getPosition();
            Vector2 nextPoint = parts[next].getPosition();

            final float segmentLength = Vector2.distance(currPoint, nextPoint);
            accLength += segmentLength;

            if (Vector2.distance(currPoint, nextPoint) < 10e-7) {
                continue;
            }

            if (!Vector2.isPerpendicularToSegment(point, currPoint, nextPoint)) {
                continue;
            }

            float distanceToLine = Vector2.distanceToLine(point, currPoint, nextPoint);
            if (distanceToLine < thicknessAtLength(accLength) / 2.0f) {
                return true;
            }
        }

        return false;
    }


    public Vector2 getPositionAtLength(float length) {
        int curr = headPointer;
        int next = nextPointer(headPointer);
        float accLength = 0.0f;

        while (next != tailPointer) {
            float segmentLength = Vector2.distance(parts[curr].getPosition(), parts[next].getPosition());

            if (accLength + segmentLength >= length) {
                float diff = length - accLength;
                Vector2 segmentVector = Vector2.minus(parts[next].getPosition(), parts[curr].getPosition());
                return Vector2.plus(parts[curr].getPosition(), segmentVector.withLength(diff));
            }

            accLength += segmentLength;
            curr = next;
            next = nextPointer(next);
        }

        return tailPosition();
    }


    public Vector2 tailPosition() {
        return new Vector2((parts[prevPointer(tailPointer)].getPosition()));
    }


    public float distanceFrom(Vector2 point) {
        float minDistance = Float.MAX_VALUE;

        int curr = headPointer;
        int next = nextPointer(headPointer);

        for ( ; next != tailPointer; curr = next, next = nextPointer(next)) {
            Vector2 currPoint = parts[curr].getPosition();
            Vector2 nextPoint = parts[next].getPosition();

            if (Vector2.distance(currPoint, nextPoint) < 10e-7) {
                continue;
            }

            if (!Vector2.isPerpendicularToSegment(point, currPoint, nextPoint)) {
                continue;
            }

            float distanceToLine = Vector2.distanceToLine(point, currPoint, nextPoint);
            minDistance = Math.min(distanceToLine, minDistance);
        }

        return minDistance;
    }

    public static final float TAIL_TAPER_PERCENTAGE = 0.1f;

    public float thicknessAtLength(float length) {
        if (length > getLength()) {
            return -1;
        }

        if (length < (1.0f - TAIL_TAPER_PERCENTAGE) * getLength()) {
            return getThickness();
        }

        final float distanceFromTail = getLength() - length;
        final float taperLength = getLength() * TAIL_TAPER_PERCENTAGE;

        return getThickness() * distanceFromTail / taperLength;
    }


    public void addScore(int points) {
        score += points;
    }


    public float getEatDistance() {
        return getThickness() * EAT_DISTANCE_RATIO;
    }


    /**
     * Returns the players score capped at the growth limit.
     * @return The capped score.
     */
    public float cappedScore() {
        return Math.min(GROWTH_CAP, getScore());
    }


    /**
     * Gets the length of the snake.
     * @return The length.
     */
    public float getLength() {
        return MIN_LENGTH + GROWTH_RATE * cappedScore();
    }


    public static float maxLength() {
        return MIN_LENGTH + GROWTH_RATE * GROWTH_CAP;
    }


    /**
     * Get the thickness of the snake.
     * @return The thickness.
     */
    public float getThickness() {
        return MIN_THICKNESS + GROWTH_RATE * (float) Math.sqrt(cappedScore());
    }


    /**
     * Get the first {@link SnakePart}
     * @return The head.
     */
    public SnakePart getHead() {
        return parts[getHeadPointer()];
    }


    /**
     * Get the spatial position of the snake's head.
     * @return Vector2 position.
     */
    public Vector2 getHeadPosition() {
        return getHead().getPosition();
    }


    private Vector2 direction = Vector2.up();
    private Vector2 moveVec = Vector2.zero();

    /**
     * Move the snake.
     * @param desiredMove The direction in which the player wishes to move.
     */
    public void move(Vector2 desiredMove, float dt) {
        incrementHeadPointer();

        if (isTurbo()) {
            burnScore(dt);
        }

        if (desiredMove.magnitude() > 0.0f) {
            direction.rotateTowards(desiredMove, turnSpeed() * dt);
        }

        moveVec.set(direction);
        getHeadPosition().add(moveVec.multiply(moveSpeed() * dt));

        updateTailPointer();
        updateBoundingBox();
    }


    private void incrementHeadPointer() {
        if (headPointer == nextPointer(tailPointer)) {
            return;
        }

        Vector2 prevHeadPosition = getHeadPosition();
        headPointer = prevPointer(headPointer);
        getHeadPosition().set(prevHeadPosition);
    }


    private void burnScore(float dt) {
        score -= dt * BURN_SPEED;
        score = Math.max(0, score);
    }


    private void dumpSnakeParts() {
        System.out.print("Snake: [ ");
        for (int i = headPointer; i != tailPointer; i = nextPointer(i)) {
            if (i != headPointer) {
                System.out.print(", ");
            }
            System.out.print(parts[i].getPosition());
        }
        System.out.println(" ]");
    }


    private void updateTailPointer() {
        final float length = getLength();
        float partDistance = 0;

        int i = 0;
        int curr = headPointer;
        int next = nextPointer(headPointer);

        while (next != tailPointer) {
            if (i > 2 && partDistance > length + TAIL_SLACK) {
                tailPointer = next;
                return;
            }

            partDistance += Vector2.distance(parts[curr].getPosition(), parts[next].getPosition());

            i++;
            curr = next;
            next = nextPointer(next);
        }
    }


    private float segmentAngle() {
        final Vector2 headPosition = getHeadPosition();
        final Vector2 neckPosition = getNeckPosition();
        final Vector2 prevPosition = getPart(2).getPosition();

        final Vector2 headVec = Vector2.minus(headPosition, neckPosition);
        final Vector2 neckVec = Vector2.minus(neckPosition, prevPosition);

        return (float) Vector2.angleBetween(neckVec, headVec);
    }


    private SnakePart getPart(int i) {
        return parts[(headPointer + i) % MAX_PARTS];
    }


    private float moveSpeed() {
        return isTurbo() ? turboMoveSpeed() : normalMoveSpeed();
    }


    private float turboMoveSpeed() {
        return TURBO_BOOST_FACTOR * normalMoveSpeed();
    }


    private float normalMoveSpeed() {
        return MOVE_SPEED;
    }


    private float turnSpeed() {
        return (2.0f * PI * MOVE_SPEED) / (TURN_RADIUS_FACTOR * PI * getThickness());
    }


    /**
     * Gets the position of the second snake part.
     * @return Vector2 position.
     */
    private Vector2 getNeckPosition() {
        return getNeck().getPosition();
    }


    /**
     * Get the second snake part.
     * @return
     */
    private SnakePart getNeck() {
        return parts[getNeckPointer()];
    }

    private int getNeckPointer() {
        return nextPointer(headPointer);
    }

    private int nextPointer(int pointer) {
        return mod(pointer + 1, MAX_PARTS);
    }

    private int prevPointer(int pointer) {
        return mod(pointer - 1, MAX_PARTS);
    }

    private int mod(int x, int radix) {
        int rem = x % radix;
        return rem < 0 ? rem + radix : rem;
    }

    public Rect getBoundingBox() {
        return boundingBox;
    }

    public void setBoundingBox(Rect boundingBox) {
        this.boundingBox = boundingBox;
    }

    public int getPid() {
        return pid;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getSkin() {
        return skin;
    }

    public void setSkin(int skin) {
        this.skin = skin;
    }

    public int getScore() {
        return score;
    }

    public void setScore(int score) {
        this.score = score;
    }

    public boolean isTurbo() {
        return isTurbo && getScore() > MIN_TURBO_SCORE;
    }

    public void setTurbo(boolean turbo) {
        isTurbo = turbo;
    }

    public boolean isDead() {
        return isDead;
    }

    public void setDead(boolean dead) {
        isDead = dead;
    }

    public SnakePart[] getParts() {
        return parts;
    }

    public int getHeadPointer() {
        return headPointer;
    }

    public void setHeadPointer(int headPointer) {
        this.headPointer = headPointer;
    }

    public int getTailPointer() {
        return tailPointer;
    }

    public void setTailPointer(int tailPointer) {
        this.tailPointer = tailPointer;
    }
}

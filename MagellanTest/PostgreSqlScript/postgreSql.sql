CREATE DATABASE Part;

-- Connect to the Part database
\c Part;

-- Create Table
CREATE TABLE item (
	id SERIAL PRIMARY KEY,
	item_name VARCHAR(50) NOT NULL,
	parent_item INTEGER REFERENCES item(id),
	cost INTEGER NOT NULL,
	req_date DATE NOT NULL
);

-- Insert Data
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES 
('Item1', null, 500, '2024-02-20'),
('Sub1', 1, 200, '2024-02-10'),
('Sub2', 1, 300, '2024-01-05'),
('Sub3', 2, 300, '2024-01-02'),
('Sub4', 2, 400, '2024-01-02'),
('Item2', null, 600, '2024-03-15'),
('Sub1', 6, 200, '2024-02-25');

-- Function to Get Total Cost
CREATE OR REPLACE FUNCTION Get_Total_Cost(input_item_name VARCHAR(50)) RETURNS INTEGER AS $$
DECLARE
	total_cost INTEGER := 0;
BEGIN
	WITH RECURSIVE item_calculate AS (
        SELECT id, item_name, parent_item, cost
        FROM item
        WHERE item_name = input_item_name
        UNION ALL
        SELECT i.id, i.item_name, i.parent_item, i.cost
        FROM item i
        JOIN item_calculate ih ON ih.id = i.parent_item
    )
    SELECT SUM(cost) INTO total_cost
    FROM item_calculate;

	RETURN total_cost;

END;
$$ LANGUAGE plpgsql;

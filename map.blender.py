import bpy
import io
import os
import math
random_seed=1337

steps = []

class Connection:
    def __init__(self, id, cost):
        self.id = id
        self.cost = cost

class Step:
    def __init__(self, id, x, y):
        self.id = id
        self.x = x
        self.y = y
        self.connections = []
        
def get_step(vert):
    for step in steps:
        if step.x == vert.co.x and step.y == vert.co.y:
            return step

def get_distance(step1, step2):
    distx = step1.x - step2.x
    disty = step1.y - step2.y
    return math.sqrt(abs(distx*distx + disty*disty))
        
def grid_to_wireframe(thinkness):
    # Apply wireframe modifier
    bpy.ops.object.modifier_add(type='WIREFRAME')
    bpy.context.object.modifiers["Wireframe"].thickness = thinkness
    bpy.context.object.modifiers["Wireframe"].use_boundary = True
    bpy.ops.object.modifier_apply(apply_as='DATA', modifier="Wireframe")

    # Make wireframe flat, and remove doubles
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.transform.resize(value=(1, 1, 0), orient_type='GLOBAL', orient_matrix=((1, 0, 0), (0, 1, 0), (0, 0, 1)), orient_matrix_type='GLOBAL', constraint_axis=(False, False, True), mirror=True, use_proportional_edit=False, proportional_edit_falloff='SMOOTH', proportional_size=1, use_proportional_connected=False, use_proportional_projected=False)
    bpy.ops.mesh.remove_doubles()

    # Recalculate normals
    bpy.ops.mesh.normals_make_consistent(inside=False)
    
    # Switch to object mode
    bpy.ops.object.mode_set(mode="OBJECT")

def rename_selected(name):
    bpy.context.active_object.name = name
    
def select_object(name):
    bpy.ops.object.select_all(action='DESELECT')
    obj = bpy.data.objects[name]
    obj.select_set(state=True)
    bpy.context.view_layer.objects.active = obj

def make_selected_fat(thinkness, center):
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate = { "value": (0, 0, thinkness) })
    if center:
        bpy.ops.mesh.select_all(action='SELECT')
        bpy.ops.transform.translate(value=(0, 0, -(thinkness/2)))
    bpy.ops.object.mode_set(mode="OBJECT")

def carve_object(obj, carve):
    select_object(obj)
    bpy.ops.object.modifier_add(type='BOOLEAN')
    bpy.context.object.modifiers["Boolean"].object = bpy.data.objects[carve]
    bpy.ops.object.modifier_apply(apply_as='DATA', modifier="Boolean")
    select_object(carve)
    bpy.ops.object.delete(use_global=False, confirm=False)

# Clean up scene
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False, confirm=False)

# Create grid
bpy.ops.mesh.primitive_grid_add(x_subdivisions=10, y_subdivisions=10, size=20, enter_editmode=True, location=(0, 0, 0))

# Randomize verticies and make them flat
bpy.ops.transform.vertex_random(offset=0.8, seed=random_seed)
bpy.ops.transform.resize(value=(1, 1, 0), orient_type='GLOBAL', orient_matrix=((1, 0, 0), (0, 1, 0), (0, 0, 1)), orient_matrix_type='GLOBAL', constraint_axis=(False, False, True), mirror=True, use_proportional_edit=False, proportional_edit_falloff='SMOOTH', proportional_size=1, use_proportional_connected=False, use_proportional_projected=False)

# Apply transformation for good measure
bpy.ops.object.mode_set(mode="OBJECT")
bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)

# Rename
rename_selected("grid")

# Dissolve random verts
bpy.ops.object.mode_set(mode="EDIT")
bpy.ops.mesh.select_all(action='DESELECT')
bpy.ops.mesh.select_random(percent=20, seed=random_seed)
bpy.ops.mesh.dissolve_verts()

# Add steps to array
bpy.ops.object.mode_set(mode="OBJECT")
obj = bpy.context.active_object

for i, vert in enumerate(obj.data.vertices):
    steps.append(Step(i+1, vert.co.x, vert.co.y))

mesh = obj.data
for poly in mesh.polygons:
    for index in poly.loop_indices:
        edge = mesh.edges[mesh.loops[index].edge_index]
        step1 = get_step(mesh.vertices[edge.vertices[0]])
        step2 = get_step(mesh.vertices[edge.vertices[1]])
        dist = get_distance(step1, step2)
        step1.connections.append(Connection(step2.id, dist))
        step2.connections.append(Connection(step1.id, dist))
        

# Select main object again
select_object("grid")

# Duplicate grid
bpy.ops.object.duplicate()
rename_selected("grid_carve")

# Duplicate grid
bpy.ops.object.duplicate()
rename_selected("roads")

# Duplicate grid
bpy.ops.object.duplicate()
rename_selected("pavement")

# Building space
select_object("grid_carve")
grid_to_wireframe(1)
make_selected_fat(2, True)
carve_object("grid", "grid_carve")

# Pavements
select_object("pavement")
grid_to_wireframe(0.5)
make_selected_fat(0.05, False)

# Roads
select_object("roads")
grid_to_wireframe(0.2)

# Cut pavement
select_object("roads")
bpy.ops.object.duplicate()
rename_selected("roads_carve")
make_selected_fat(2, True)
carve_object("pavement", "roads_carve")

# Write map.json file
with open(os.path.join(os.getcwd(), "data/map.json"), "w") as file:
    file.write('{\n    "nodes": [\n')
    for i, step in enumerate(steps):
        next_char = ','
        
        if i == len(steps)-1:
            next_char = ''
        
        is_step = "true"
        
        if i % 4 == 0:
            is_step = "false"
        
        connection_string = ''
        for conn in step.connections:
            connection_string += '{{ "id": {}, "cost": {} }},'.format(conn.id, conn.cost);
        file.write('        {{ "id": {}, "x": {}, "y": {}, "connections": [{}], "step": {} }}{}\n'.format(step.id, step.x, step.y, connection_string[:-1], is_step, next_char))
    file.write("    ]\n}")

bpy.ops.export_scene.gltf(filepath=os.path.join(os.getcwd(), "client/public/assets", "city.glb"))
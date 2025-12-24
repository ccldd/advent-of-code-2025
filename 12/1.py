import sys
from dataclasses import dataclass
from typing import Dict, List
from io import StringIO

f = open(sys.argv[1])


@dataclass
class Region:
    width: int
    length: int
    quantities: List[int]

    def area(self):
        return width * length


shapes: Dict[int, List[str]] = {}
regions: List[Region] = []

lines = StringIO(f.read())
while True:
    line = lines.readline()
    if line == "":
        break

    if "x" in line:
        s = line.split(":")
        width = int(s[0].split("x")[0])
        length = int(s[0].split("x")[1])
        quantities = [int(x) for x in s[1].split(" ") if x]
        region = Region(width, length, quantities)
        regions.append(region)
    elif ":" in line:
        num = line.split(":")[0]
        shapes[int(num)] = []
    elif line != "\n":
        last_key = list(shapes.keys())[-1]
        shapes[last_key].append(line)

fit = 0
for region in regions:
    if (region.width // 3) * (region.length // 3) >= sum(region.quantities):
        fit += 1

print(fit)

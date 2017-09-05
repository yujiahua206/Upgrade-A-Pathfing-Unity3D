# Upgrade-A-Pathfing-Unity3D
upgrade A* pathfing,   using penatly to respersent different terrian and height

通常的A* 算法使用f=g+h 计算每个方格的fcost， 该算法使用f=g+h+p 计算fcost，其中p表示不同地形或者高度的
数值惩罚，可以用于3D环境中的路径计算。

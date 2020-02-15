require 'mini_magick'
require 'image_magick'


file_path = "./Assets/pictures/picture1.jpg"
image = MiniMagick::Image.open(file_path)
# image.resize "10x10";
# image.format "jpg"
image.write "./generated_image.jpg"
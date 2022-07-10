(script-fu-register
 "script-fu-extract-all-layers"
 "Extract all layers"
 "Script allofs to extract all layers from file and  save them as separate files. In additional it saves .txt files with data of every layer"
 "Allebi"
 "Copyright AllebiGames @ 2013"
 "September 5, 2013"
 "RGB* RGBA* INDEXED*"
 
 SF-IMAGE      "The image"     0
 SF-DRAWABLE   "The layer"     0
 
 SF-DIRNAME "Output folder" DIR-SEPARATOR

)
(script-fu-menu-register "script-fu-extract-all-layers" "<Image>/Filters/User's scripts")

 
 
 (define (morph-filename orig-name new-extension)
  (let* ((buffer (vector "" "" "")))
   (if (re-match "^(.*)[.]([^.]+)$" orig-name buffer)
    (string-append (substring orig-name 0 (car (vector-ref buffer 2))) new-extension)
    )
   )
  )
 

 
 
(define (script-fu-extract-all-layers inImage inLayer inOutFolder)
 (let* ( (num_layers 0)
         (layer_ids)
         (output_file_name "")
         (output_port 0)
        )
  
  (gimp-image-undo-group-start inImage)
  
    (set! num_layers (car (gimp-image-get-layers inImage)))
    (set! layer_ids (cdr (gimp-image-get-layers inImage)))
  
  (set! output_file_name (string-append inOutFolder "/"  (car (gimp-image-get-name inImage)) "_Layers.txt"))
  (set! output_port (open-output-file output_file_name))

  
  (while (> num_layers 0)
    (let* (
           (filename "")
           (raw_filename "")
           (layer (car (gimp-image-get-active-layer inImage)))
           (layer_offset_x 0)
           (layer_offset_y 0)
           (layer_offsets "")
          )
     
      (set! layer (car (gimp-image-get-active-layer inImage)))
     
      (set! layer_offset_x (car (gimp-drawable-offsets layer)))
      (set! layer_offset_y (cadr (gimp-drawable-offsets layer)))
      (set! layer_offsets (string-append  (number->string layer_offset_x) "_" (number->string layer_offset_y)))

       
      (set! raw_filename (string-append (car (gimp-item-get-name layer))  ".png"))
      (set! filename (string-append inOutFolder "/" raw_filename))
        
     
      (display raw_filename output_port)
      (display "\n" output_port)
      (display layer_offset_x output_port)
      (display "\n" output_port)
      (display layer_offset_y output_port)
      (display "\n" output_port)
      (display (number->string (car (gimp-drawable-width layer))) output_port)
      (display "\n" output_port)
      (display (number->string (car (gimp-drawable-height layer))) output_port)
      (display "\n" output_port)
      (display "--------- \n" output_port)
           
      
      (file-png-save-defaults RUN-NONINTERACTIVE inImage layer filename raw_filename)

      (gimp-image-remove-layer inImage layer)
    
    )
   
   (set! num_layers (- num_layers 1))
  )
  
   (close-output-port output_port)
  
  (gimp-image-undo-group-end inImage)
 )
)

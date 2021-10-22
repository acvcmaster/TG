# Cross-section

function parametros_qlearning()
    data = load('parametros_qlearning.csv')
    # 107aa105-7fa6-46b0-8711-f03b9fdc0981
    # ef = 0.7, df = 0.9

    # 10, 21, 32, ... 120
    x = data(10:11:120, 1);
    z = data(10:11:120, 4)
    plot(x, z);
    pause();
endfunction
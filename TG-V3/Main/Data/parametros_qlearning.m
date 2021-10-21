function parametros_qlearning()
    data = load('parametros_qlearning.csv')
    n = 11;

    # Transforma a matriz 121x1 em 11x11
    x = reshape(data(:, 1), n, n);
    y = reshape(data(:, 2), n, n);
    z = reshape(data(:, 4), n, n);
    mesh(x, y, z);
    pause();
endfunction
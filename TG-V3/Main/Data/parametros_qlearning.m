function parametros_qlearning()
    data = load('parametros_qlearning.csv')

    m = 11;
    n = 11;

    # Evenly spaced axis, hacked to this dataset 
    x = reshape(data(:,1),m,n);
    y = reshape(data(:,2),m,n);    % might be reshape(data(:,2),n,m)
    z = reshape(data(:,4),m,n);
    mesh(x',y',z');
endfunction